using BuyCrypt.Application.Helper;
using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyCrypt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyDashboard()
        {
            var userId = User.GetUserId();

            var dashboard =
                await _dashboardService.GetDashboardAsync(userId);

            if (dashboard == null)
                return NotFound(new { message = "User not found" });

            return Ok(dashboard);
        }
    }
}
