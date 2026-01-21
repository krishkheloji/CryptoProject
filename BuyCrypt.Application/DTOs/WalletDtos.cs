namespace BuyCrypt.Application.DTOs
{
    public class WalletResponse
    {
        public Guid WalletId { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "INR";
    }

    public class WalletAmountRequest
    {
        public decimal Amount { get; set; }
    }
}
