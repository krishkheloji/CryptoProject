using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using BuyCrypt.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BuyCrypt.Infrastructure.Services
{
    public class CryptoMarketService : ICryptoMarketService
    {
        private readonly HttpClient _httpClient;
        private readonly CoinGeckoOptions _options;

        public CryptoMarketService(
            HttpClient httpClient,
            IOptions<CoinGeckoOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            _httpClient.BaseAddress = new Uri(_options.BaseUrl);

            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Add(
                    "x-cg-demo-api-key", _options.ApiKey);
            }
        }


        private decimal GetDecimalSafe(JsonElement element, string property)
        {
            if (element.TryGetProperty(property, out var value))
            {
                if (value.ValueKind == JsonValueKind.Number)
                {
                    if (value.TryGetDecimal(out var dec))
                        return dec;

                    if (value.TryGetDouble(out var dbl))
                        return (decimal)dbl;
                }
            }
            return 0m;
        }

        private decimal GetNestedDecimalSafe(JsonElement element, string parent, string child)
        {
            if (element.TryGetProperty(parent, out var parentEl) &&
                parentEl.TryGetProperty(child, out var value) &&
                value.ValueKind == JsonValueKind.Number)
            {
                return value.GetDecimal();
            }
            return 0m;
        }


        public async Task<decimal> GetCurrentPriceAsync(string coinId)
        {
            var response = await _httpClient.GetAsync(
                $"simple/price?ids={coinId}&vs_currencies=inr");

            if (!response.IsSuccessStatusCode)
                return 0;

            using var doc = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            if (doc.RootElement.TryGetProperty(coinId, out var coin))
            {
                return GetDecimalSafe(coin, "inr");
            }

            return 0;
        }


        public async Task<Dictionary<string, decimal>> GetMultiplePricesAsync(List<string> coinIds)
        {
            var result = new Dictionary<string, decimal>();
            if (!coinIds.Any()) return result;

            var ids = string.Join(",", coinIds);

            var response = await _httpClient.GetAsync(
                $"simple/price?ids={ids}&vs_currencies=inr");

            if (!response.IsSuccessStatusCode)
                return result;

            using var doc = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            foreach (var coinId in coinIds)
            {
                if (doc.RootElement.TryGetProperty(coinId, out var coin))
                    result[coinId] = GetDecimalSafe(coin, "inr");
                else
                    result[coinId] = 0;
            }

            return result;
        }


        public async Task<Dictionary<string, CoinMarketData>> GetCoinDetailsAsync(List<string> coinIds)
        {
            var result = new Dictionary<string, CoinMarketData>();
            if (!coinIds.Any()) return result;

            var ids = string.Join(",", coinIds);

            var response = await _httpClient.GetAsync(
                $"simple/price?ids={ids}&vs_currencies=inr&include_24hr_change=true");

            if (!response.IsSuccessStatusCode)
                return result;

            using var doc = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            foreach (var coinId in coinIds)
            {
                if (doc.RootElement.TryGetProperty(coinId, out var coin))
                {
                    result[coinId] = new CoinMarketData
                    {
                        CoinId = coinId,
                        CurrentPrice = GetDecimalSafe(coin, "inr"),
                        PriceChange24h = GetDecimalSafe(coin, "inr_24h_change"),
                        PriceChangePercentage24h = GetDecimalSafe(coin, "inr_24h_change")
                    };
                }
            }

            return result;
        }



        public async Task<List<CoinListItem>> GetTopCoinsAsync(int limit = 100)
        {
            var response = await _httpClient.GetAsync(
                $"coins/markets?vs_currency=inr&order=market_cap_desc&per_page={limit}&page=1&sparkline=false&price_change_percentage=24h");

            if (!response.IsSuccessStatusCode)
                return new List<CoinListItem>();

            using var doc = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var coins = new List<CoinListItem>();

            foreach (var item in doc.RootElement.EnumerateArray())
            {
                coins.Add(new CoinListItem
                {
                    Id = item.GetProperty("id").GetString() ?? "",
                    Symbol = item.GetProperty("symbol").GetString()?.ToUpper() ?? "",
                    Name = item.GetProperty("name").GetString() ?? "",
                    Image = item.GetProperty("image").GetString() ?? "",

                    CurrentPrice = GetDecimalSafe(item, "current_price"),
                    MarketCap = GetDecimalSafe(item, "market_cap"),
                    PriceChange24h = GetDecimalSafe(item, "price_change_percentage_24h")
                });
            }

            return coins;
        }


        public async Task<CoinDetailInfo> GetCoinDetailAsync(string coinId)
        {
            var response = await _httpClient.GetAsync(
                $"coins/{coinId}?localization=false&tickers=false&community_data=false&developer_data=false");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch coin details");

            using var doc = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var root = doc.RootElement;
            var marketData = root.GetProperty("market_data");

            return new CoinDetailInfo
            {
                Id = root.GetProperty("id").GetString() ?? "",
                Symbol = root.GetProperty("symbol").GetString()?.ToUpper() ?? "",
                Name = root.GetProperty("name").GetString() ?? "",
                Image = root.GetProperty("image").GetProperty("large").GetString() ?? "",

                CurrentPrice = GetNestedDecimalSafe(marketData, "current_price", "inr"),
                MarketCap = GetNestedDecimalSafe(marketData, "market_cap", "inr"),
                High24h = GetNestedDecimalSafe(marketData, "high_24h", "inr"),
                Low24h = GetNestedDecimalSafe(marketData, "low_24h", "inr"),

                PriceChange24h = GetDecimalSafe(marketData, "price_change_percentage_24h"),
                PriceChangePercentage24h = GetDecimalSafe(marketData, "price_change_percentage_24h")
            };
        }
    }
}
