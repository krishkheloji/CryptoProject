using BuyCrypt.Application.DTOs;

namespace BuyCrypt.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<FavoriteResponse> AddFavoriteAsync(Guid userId, AddFavoriteRequest request);
        Task<FavoriteListResponse> GetUserFavoritesAsync(Guid userId);
        Task RemoveFavoriteAsync(Guid userId, Guid favoriteId);
    }
}
