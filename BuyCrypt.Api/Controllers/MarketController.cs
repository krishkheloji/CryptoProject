using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/market")]
public class MarketController : ControllerBase
{
    private readonly ICryptoMarketService _marketService;

    public MarketController(ICryptoMarketService marketService)
    {
        _marketService = marketService;
    }

    [HttpGet("coins")]
    public async Task<IActionResult> GetTopCoins([FromQuery] int limit = 100)
    {
        if (limit <= 0 || limit > 250)
            return BadRequest("Limit must be between 1 and 250");

        var coins = await _marketService.GetTopCoinsAsync(limit);
        return Ok(coins);
    }

    [HttpGet("coins/{coinId}")]
    public async Task<IActionResult> GetCoinDetail(string coinId)
    {
        if (string.IsNullOrWhiteSpace(coinId))
            return BadRequest("Invalid coin id");

        try
        {
            var coin = await _marketService.GetCoinDetailAsync(coinId);
            return Ok(coin);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Coin not found" });
        }
        catch
        {
            return StatusCode(503, new { message = "Market service unavailable" });
        }
    }
}
