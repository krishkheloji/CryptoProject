using AutoMapper;
using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using BuyCrypt.Domain.Enums;
using BuyCrypt.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BuyCrypt.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly CryptoDbContext _context;
        private readonly IMapper _mapper;

        public TransactionService(
            CryptoDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TransactionResponse?> GetTransactionAsync(
            Guid transactionId,
            Guid userId)
        {
            var txn = await _context.Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t =>
                    t.TransactionId == transactionId &&
                    t.UserId == userId);

            return txn == null
                ? null
                : _mapper.Map<TransactionResponse>(txn);
        }

        public async Task<TransactionHistoryResponse> GetUserTransactionsAsync(
            Guid userId)
        {
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return new TransactionHistoryResponse
            {
                UserId = userId,
                TotalTransactions = transactions.Count,

                TotalBuyAmount = transactions
                    .Where(t => t.TransactionType == TransactionType.BUY)
                    .Sum(t => t.TotalAmount),

                TotalSellAmount = transactions
                    .Where(t => t.TransactionType == TransactionType.SELL)
                    .Sum(t => t.TotalAmount),

                Transactions = _mapper.Map<List<TransactionResponse>>(transactions)
            };
        }

        public async Task<List<TransactionResponse>> GetWalletTransactionsAsync(
            Guid userId,
            Guid walletId)
        {
            var walletExists = await _context.Wallets
                .AnyAsync(w =>
                    w.WalletId == walletId &&
                    w.UserId == userId);

            if (!walletExists)
                throw new UnauthorizedAccessException(
                    "You do not have access to this wallet");

            var txns = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();

            return _mapper.Map<List<TransactionResponse>>(txns);
        }
    }
}
