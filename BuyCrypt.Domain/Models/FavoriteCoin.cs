namespace BuyCrypt.Domain.Models
{
    public class FavoriteCoin
    {
        public Guid FavoriteId { get; private set; }
        public Guid UserId { get; private set; }
        public string CoinId { get; private set; } = null!;
        public string Symbol { get; private set; } = null!;
        public string CoinName { get; private set; } = null!;
        public DateTime AddedAt { get; private set; }

        public UserProfile User { get; private set; } = null!;

        private FavoriteCoin() { }

        public FavoriteCoin(
            Guid userId,
            string coinId,
            string symbol,
            string coinName)
        {
            FavoriteId = Guid.NewGuid();
            UserId = userId;
            CoinId = coinId;
            Symbol = symbol.ToUpper();
            CoinName = coinName;
            AddedAt = DateTime.UtcNow;
        }
    }
}
