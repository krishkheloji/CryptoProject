namespace BuyCrypt.Domain.Models
{
    public class Wallet
    {
        public Guid WalletId { get; private set; }
        public Guid UserId { get; private set; }
        public decimal Balance { get; private set; }
        public string Currency { get; private set; } = "INR";
        public DateTime CreatedAt { get; private set; }

        public UserProfile User { get; private set; } = null!;
        public ICollection<Transaction> Transactions { get; private set; }
            = new List<Transaction>();

        private Wallet() { }

        public Wallet(Guid userId)
        {
            WalletId = Guid.NewGuid();
            UserId = userId;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Invalid deposit amount");

            Balance += amount;
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Invalid withdrawal amount");

            if (Balance < amount)
                throw new Exception("Insufficient balance");

            Balance -= amount;
        }





    }
}
