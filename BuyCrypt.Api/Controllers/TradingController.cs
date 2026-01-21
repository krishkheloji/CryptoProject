using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Helper;
using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyCrypt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/trading")]
    public class TradingController : ControllerBase
    {
        private readonly ITradingService _tradingService;

        public TradingController(ITradingService tradingService)
        {
            _tradingService = tradingService;
        }

        [HttpPost("buy")]
        public async Task<IActionResult> BuyAsset(BuyAssetRequest request)
        {
            var userId = User.GetUserId();
            return Ok(await _tradingService.BuyAssetAsync(userId, request));
        }

        [HttpPost("sell")]
        public async Task<IActionResult> SellAsset(SellAssetRequest request)
        {
            var userId = User.GetUserId();
            return Ok(await _tradingService.SellAssetAsync(userId, request));
        }
    }
}
