using BuyCrypt.Application.DTOs;

namespace BuyCrypt.Application.Interfaces
{
    public interface ITradingService
    {
        Task<TradeResponse> BuyAssetAsync(Guid userId, BuyAssetRequest request);
        Task<TradeResponse> SellAssetAsync(Guid userId, SellAssetRequest request);
    }
}
