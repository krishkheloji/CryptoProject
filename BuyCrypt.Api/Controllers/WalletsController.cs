using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BuyCrypt.Application.Helper;
using Microsoft.AspNetCore.Authorization;

namespace BuyCrypt.Api.Controllers
{ 
    [Authorize]
    [ApiController]
    [Route("api/wallets")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetWallet()
        {
            var userId = User.GetUserId();
            return Ok(await _service.GetOrCreateAsync(userId));
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit(WalletAmountRequest request)
        {
            var userId = User.GetUserId();
            return Ok(await _service.DepositAsync(userId, request.Amount));
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw(WalletAmountRequest request)
        {
            var userId = User.GetUserId();
            return Ok(await _service.WithdrawAsync(userId, request.Amount));
        }
    }

}
