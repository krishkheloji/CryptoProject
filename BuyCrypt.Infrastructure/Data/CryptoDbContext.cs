using BuyCrypt.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BuyCrypt.Infrastructure.Data
{
    public class CryptoDbContext : DbContext
    {
        public CryptoDbContext(DbContextOptions<CryptoDbContext> options)
            : base(options) { }

        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<PortfolioAsset> PortfolioAssets => Set<PortfolioAsset>();
        public DbSet<FavoriteCoin> FavoriteCoins => Set<FavoriteCoin>();
        public DbSet<PortfolioSnapshot> PortfolioSnapshots => Set<PortfolioSnapshot>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(x => x.UserId);
                entity.HasIndex(x => x.Email).IsUnique();

                entity.Property(x => x.FullName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.Email)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.PhoneNumber)
                      .HasMaxLength(20);

                entity.Property(x => x.PreferredCurrency)
                      .HasMaxLength(10)
                      .HasDefaultValue("INR");

                entity.HasMany(x => x.Wallets)
                      .WithOne(x => x.User)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.PortfolioAssets)
                      .WithOne(x => x.User)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.Transactions)
                      .WithOne(x => x.User)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.FavoriteCoins)
                      .WithOne(x => x.User)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(x => x.PortfolioSnapshots)
                      .WithOne(x => x.User)
                      .HasForeignKey(x => x.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(x => x.WalletId);
                entity.HasIndex(x => x.UserId).IsUnique();

                entity.Property(x => x.Balance)
                      .HasPrecision(18, 2)
                      .IsRequired();

                entity.Property(x => x.Currency)
                      .HasMaxLength(10)
                      .HasDefaultValue("INR");

                entity.HasMany(x => x.Transactions)
                      .WithOne(x => x.Wallet)
                      .HasForeignKey(x => x.WalletId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PortfolioAsset>(entity =>
            {
                entity.HasKey(x => x.AssetId);
                entity.HasIndex(x => new { x.UserId, x.CoinId });

                entity.Property(x => x.CoinId)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.Symbol)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(x => x.CoinName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(x => x.Quantity)
                      .HasPrecision(38, 18);

                entity.Property(x => x.AvgBuyPrice)
                      .HasPrecision(18, 2);

                entity.Property(x => x.TotalInvested)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(x => x.TransactionId);

                entity.Property(x => x.CoinId)
                      .HasMaxLength(100)
                      .IsRequired(false);   

                entity.Property(x => x.Symbol)
                      .HasMaxLength(20)
                      .IsRequired(false);  

                entity.Property(x => x.TransactionType)
                      .HasConversion<string>()
                      .HasMaxLength(10)
                      .IsRequired();

                entity.Property(x => x.Quantity)
                      .HasPrecision(38, 18);

                entity.Property(x => x.PricePerCoin)
                      .HasPrecision(18, 2);

                entity.Property(x => x.TotalAmount)
                      .HasPrecision(18, 2);

                entity.Property(x => x.Fees)
                      .HasPrecision(18, 2);

                entity.Property(x => x.Notes)
                      .HasMaxLength(500);
            });


            modelBuilder.Entity<FavoriteCoin>(entity =>
            {
                entity.HasKey(x => x.FavoriteId);
                entity.HasIndex(x => new { x.UserId, x.CoinId }).IsUnique();

                entity.Property(x => x.CoinId)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.Symbol)
                      .IsRequired()
                      .HasMaxLength(20);

                entity.Property(x => x.CoinName)
                      .IsRequired()
                      .HasMaxLength(200);
            });

            modelBuilder.Entity<PortfolioSnapshot>(entity =>
            {
                entity.HasKey(x => x.SnapshotId);
                entity.HasIndex(x => new { x.UserId, x.SnapshotDate }).IsUnique();

                entity.Property(x => x.TotalInvested)
                      .HasPrecision(18, 2);

                entity.Property(x => x.CurrentValue)
                      .HasPrecision(18, 2);

                entity.Property(x => x.ProfitLoss)
                      .HasPrecision(18, 2);

                entity.Property(x => x.ProfitLossPercentage)
                      .HasPrecision(10, 2);
            });
        }
    }
}
