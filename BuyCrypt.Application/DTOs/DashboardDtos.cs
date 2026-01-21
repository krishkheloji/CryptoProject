using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyCrypt.Application.DTOs
{
    public class DashboardResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;

        // Portfolio Stats
        public decimal TotalInvested { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalProfitLoss { get; set; }
        public decimal ProfitLossPercentage { get; set; }

        public decimal NetWorth { get; set; }  
        public decimal AvailableCash { get; set; }  
        public decimal InvestedPercentage { get; set; }  

        public decimal TotalWalletBalance { get; set; }
        public int TotalAssets { get; set; }
        public int TotalTransactions { get; set; }

        public List<AssetHoldingResponse> Holdings { get; set; } = new();
        public List<WalletSummary> Wallets { get; set; } = new();

        public DateTime LastUpdated { get; set; }
    }

    public class AssetHoldingResponse
    {
        public Guid AssetId { get; set; }  
        public string CoinId { get; set; } = null!;
        public string Symbol { get; set; } = null!;
        public string CoinName { get; set; } = null!;
        public decimal Quantity { get; set; }
        public decimal AvgBuyPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal InvestedValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercentage { get; set; }
        public decimal PortfolioWeightage { get; set; }
    }

    public class WalletSummary
    {
        public Guid WalletId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "INR";
    }

}
