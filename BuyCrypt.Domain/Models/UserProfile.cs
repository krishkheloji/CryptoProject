using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BuyCrypt.Domain.Models
{
    public class UserProfile
    {
        public Guid UserId { get; private set; }
        public string FullName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }
        public string PreferredCurrency { get; private set; } = "INR";
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public ICollection<Wallet> Wallets { get; private set; } = new List<Wallet>();
        public ICollection<PortfolioAsset> PortfolioAssets { get; private set; } = new List<PortfolioAsset>();
        public ICollection<Transaction> Transactions { get; private set; } = new List<Transaction>();
        public ICollection<FavoriteCoin> FavoriteCoins { get; private set; } = new List<FavoriteCoin>();
        public ICollection<PortfolioSnapshot> PortfolioSnapshots { get; private set; } = new List<PortfolioSnapshot>();
    }
}
