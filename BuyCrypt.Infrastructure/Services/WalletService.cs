using AutoMapper;
using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using BuyCrypt.Domain.Models;
using BuyCrypt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuyCrypt.Infrastructure.Services
{
    using BuyCrypt.Application.DTOs;
    using BuyCrypt.Domain.Models;
    using BuyCrypt.Infrastructure.Data;
    using Microsoft.EntityFrameworkCore;

    public class WalletService : IWalletService
    {
        private readonly CryptoDbContext _context;

        public WalletService(CryptoDbContext context)
        {
            _context = context;
        }

        public async Task<WalletResponse> GetOrCreateAsync(Guid userId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (wallet == null)
            {
                wallet = new Wallet(userId);
                _context.Wallets.Add(wallet);
                await _context.SaveChangesAsync();
            }

            return new WalletResponse
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance,
                Currency = wallet.Currency
            };
        }

        public async Task<WalletResponse> DepositAsync(Guid userId, decimal amount)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(x => x.UserId == userId)
                ?? throw new Exception("Wallet not found");

            wallet.Deposit(amount);

            var transaction = Transaction.CreateDeposit(
                userId,
                wallet.WalletId,
                amount
            );

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new WalletResponse
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance,
                Currency = wallet.Currency
            };
        }

        public async Task<WalletResponse> WithdrawAsync(Guid userId, decimal amount)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(x => x.UserId == userId)
                ?? throw new Exception("Wallet not found");

            wallet.Withdraw(amount);

            var transaction = Transaction.CreateWithdraw(
                userId,
                wallet.WalletId,
                amount
            );

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return new WalletResponse
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance,
                Currency = wallet.Currency
            };
        }
    }

}
