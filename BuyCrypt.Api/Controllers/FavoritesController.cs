using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Helper;
using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyCrypt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite(AddFavoriteRequest request)
        {
            var userId = User.GetUserId();
            var favorite =
                await _favoriteService.AddFavoriteAsync(userId, request);

            return Ok(favorite);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = User.GetUserId();
            return Ok(await _favoriteService.GetUserFavoritesAsync(userId));
        }

        [HttpDelete("{favoriteId}")]
        public async Task<IActionResult> RemoveFavorite(Guid favoriteId)
        {
            var userId = User.GetUserId();
            await _favoriteService.RemoveFavoriteAsync(userId, favoriteId);
            return NoContent();
        }
    }
}
