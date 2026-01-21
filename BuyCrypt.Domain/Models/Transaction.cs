using BuyCrypt.Domain.Enums;

namespace BuyCrypt.Domain.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid WalletId { get; private set; }

        public string? CoinId { get; private set; }
        public string? Symbol { get; private set; }

        public TransactionType TransactionType { get; private set; }

        public decimal Quantity { get; private set; }
        public decimal PricePerCoin { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal Fees { get; private set; }

        public string? Notes { get; private set; }
        public DateTime TransactionDate { get; private set; }

        public Wallet Wallet { get; private set; } = null!;
        public UserProfile User { get; private set; } = null!;

        private Transaction() { }

        public static Transaction CreateDeposit(
            Guid userId,
            Guid walletId,
            decimal amount)
        {
            return new Transaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = walletId,
                TransactionType = TransactionType.DEPOSIT,
                Quantity = 0,
                PricePerCoin = 0,
                TotalAmount = amount,
                Fees = 0,
                Notes = "Wallet deposit",
                TransactionDate = DateTime.UtcNow
            };
        }

        public static Transaction CreateWithdraw(
            Guid userId,
            Guid walletId,
            decimal amount)
        {
            return new Transaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = walletId,
                TransactionType = TransactionType.WITHDRAW,
                Quantity = 0,
                PricePerCoin = 0,
                TotalAmount = amount,
                Fees = 0,
                Notes = "Wallet withdrawal",
                TransactionDate = DateTime.UtcNow
            };
        }

        public static Transaction CreateBuy(
            Guid userId,
            Guid walletId,
            string coinId,
            string symbol,
            decimal quantity,
            decimal price,
            decimal fees,
            string? notes)
        {
            return new Transaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = walletId,
                CoinId = coinId,
                Symbol = symbol,
                TransactionType = TransactionType.BUY,
                Quantity = quantity,
                PricePerCoin = price,
                TotalAmount = quantity * price,
                Fees = fees,
                Notes = notes,
                TransactionDate = DateTime.UtcNow
            };
        }

        public static Transaction CreateSell(
            Guid userId,
            Guid walletId,
            string coinId,
            string symbol,
            decimal quantity,
            decimal price,
            decimal fees,
            string? notes)
        {
            return new Transaction
            {
                TransactionId = Guid.NewGuid(),
                UserId = userId,
                WalletId = walletId,
                CoinId = coinId,
                Symbol = symbol,
                TransactionType = TransactionType.SELL,
                Quantity = quantity,
                PricePerCoin = price,
                TotalAmount = quantity * price,
                Fees = fees,
                Notes = notes,
                TransactionDate = DateTime.UtcNow
            };
        }
    }
}
