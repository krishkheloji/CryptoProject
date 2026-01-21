using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using BuyCrypt.Domain.Models;
using BuyCrypt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuyCrypt.Infrastructure.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly CryptoDbContext _context;
        private readonly ICryptoMarketService _cryptoService;

        public FavoriteService(
            CryptoDbContext context,
            ICryptoMarketService cryptoService)
        {
            _context = context;
            _cryptoService = cryptoService;
        }

        public async Task<FavoriteResponse> AddFavoriteAsync(
            Guid userId,
            AddFavoriteRequest request)
        {
            var exists = await _context.FavoriteCoins.AnyAsync(f =>
                f.UserId == userId &&
                f.CoinId == request.CoinId);

            if (exists)
                throw new InvalidOperationException("Coin already in favorites");

            var favorite = new FavoriteCoin(
                userId,
                request.CoinId,
                request.Symbol,
                request.CoinName
            );

            _context.FavoriteCoins.Add(favorite);
            await _context.SaveChangesAsync();

            var marketData =
                await _cryptoService.GetCoinDetailsAsync(
                    new List<string> { request.CoinId });

            var coin =
                marketData.ContainsKey(request.CoinId)
                    ? marketData[request.CoinId]
                    : new CoinMarketData();

            return new FavoriteResponse
            {
                FavoriteId = favorite.FavoriteId,
                CoinId = favorite.CoinId,
                Symbol = favorite.Symbol,
                CoinName = favorite.CoinName,
                CurrentPrice = coin.CurrentPrice,
                PriceChange24h = coin.PriceChange24h,
                AddedAt = favorite.AddedAt
            };
        }

        public async Task<FavoriteListResponse> GetUserFavoritesAsync(Guid userId)
        {
            var favorites = await _context.FavoriteCoins
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.AddedAt)
                .ToListAsync();

            if (!favorites.Any())
            {
                return new FavoriteListResponse
                {
                    TotalFavorites = 0,
                    Favorites = new(),
                    LastUpdated = DateTime.UtcNow
                };
            }

            var coinIds = favorites.Select(f => f.CoinId).ToList();
            var marketData =
                await _cryptoService.GetCoinDetailsAsync(coinIds);

            var response = favorites.Select(f =>
            {
                var coin =
                    marketData.ContainsKey(f.CoinId)
                        ? marketData[f.CoinId]
                        : new CoinMarketData();

                return new FavoriteResponse
                {
                    FavoriteId = f.FavoriteId,
                    CoinId = f.CoinId,
                    Symbol = f.Symbol,
                    CoinName = f.CoinName,
                    CurrentPrice = coin.CurrentPrice,
                    PriceChange24h = coin.PriceChange24h,
                    AddedAt = f.AddedAt
                };
            }).ToList();

            return new FavoriteListResponse
            {
                TotalFavorites = response.Count,
                Favorites = response,
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task RemoveFavoriteAsync(Guid userId, Guid favoriteId)
        {
            var favorite = await _context.FavoriteCoins
                .FirstOrDefaultAsync(f =>
                    f.FavoriteId == favoriteId &&
                    f.UserId == userId);

            if (favorite == null)
                throw new KeyNotFoundException("Favorite not found");

            _context.FavoriteCoins.Remove(favorite);
            await _context.SaveChangesAsync();
        }
    }
}
