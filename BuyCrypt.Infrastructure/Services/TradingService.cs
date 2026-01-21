using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using BuyCrypt.Domain.Enums;
using BuyCrypt.Domain.Models;
using BuyCrypt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuyCrypt.Infrastructure.Services
{
    public class TradingService : ITradingService
    {
        private readonly CryptoDbContext _context;
        private readonly ICryptoMarketService _cryptoService;

        private const decimal FeePercentage = 0.01m;

        public TradingService(
            CryptoDbContext context,
            ICryptoMarketService cryptoService)
        {
            _context = context;
            _cryptoService = cryptoService;
        }

        public async Task<TradeResponse> BuyAssetAsync(
            Guid userId,
            BuyAssetRequest request)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId)
                ?? throw new InvalidOperationException("Wallet not found");

            var price = await _cryptoService.GetCurrentPriceAsync(request.CoinId);
            if (price <= 0)
                throw new InvalidOperationException("Price fetch failed");

            var totalAmount = request.Quantity * price;
            var fees = totalAmount * FeePercentage;
            var totalCost = totalAmount + fees;

            wallet.Withdraw(totalCost);

            var transaction = Transaction.CreateBuy(
                userId,
                wallet.WalletId,
                request.CoinId,
                request.Symbol.ToUpper(),
                request.Quantity,
                price,
                fees,
                request.Notes
            );

            _context.Transactions.Add(transaction);

            var asset = await _context.PortfolioAssets
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    a.CoinId == request.CoinId);

            if (asset == null)
            {
                asset = new PortfolioAsset(
                    userId,
                    request.CoinId,
                    request.Symbol.ToUpper(),
                    request.CoinName,
                    request.Quantity,
                    price
                );

                _context.PortfolioAssets.Add(asset);
            }
            else
            {
                asset.AddQuantity(request.Quantity, totalAmount);
            }

            await _context.SaveChangesAsync();

            return new TradeResponse
            {
                TransactionId = transaction.TransactionId,
                TransactionType = TransactionType.BUY.ToString(),
                CoinId = request.CoinId,
                Symbol = request.Symbol,
                Quantity = request.Quantity,
                PricePerCoin = price,
                TotalAmount = totalAmount,
                Fees = fees,
                NewWalletBalance = wallet.Balance,
                TransactionDate = transaction.TransactionDate,
                Message = "Buy order executed successfully"
            };
        }

        public async Task<TradeResponse> SellAssetAsync(
            Guid userId,
            SellAssetRequest request)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId)
                ?? throw new InvalidOperationException("Wallet not found");

            var asset = await _context.PortfolioAssets
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    a.CoinId == request.CoinId)
                ?? throw new InvalidOperationException("Asset not found");

            if (asset.Quantity < request.Quantity)
                throw new InvalidOperationException("Insufficient quantity");

            var price = await _cryptoService.GetCurrentPriceAsync(request.CoinId);
            if (price <= 0)
                throw new InvalidOperationException("Price fetch failed");

            var totalAmount = request.Quantity * price;
            var fees = totalAmount * FeePercentage;
            var netAmount = totalAmount - fees;

            wallet.Deposit(netAmount);

            var transaction = Transaction.CreateSell(
                userId,
                wallet.WalletId,
                request.CoinId,
                asset.Symbol,
                request.Quantity,
                price,
                fees,
                request.Notes
            );

            _context.Transactions.Add(transaction);

            asset.RemoveQuantity(request.Quantity);
            if (asset.Quantity == 0)
                _context.PortfolioAssets.Remove(asset);

            await _context.SaveChangesAsync();

            return new TradeResponse
            {
                TransactionId = transaction.TransactionId,
                TransactionType = TransactionType.SELL.ToString(),
                CoinId = request.CoinId,
                Symbol = asset.Symbol,
                Quantity = request.Quantity,
                PricePerCoin = price,
                TotalAmount = totalAmount,
                Fees = fees,
                NewWalletBalance = wallet.Balance,
                TransactionDate = transaction.TransactionDate,
                Message = "Sell order executed successfully"
            };
        }
    }
}
