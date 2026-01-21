using BuyCrypt.Application.Helper;
using BuyCrypt.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuyCrypt.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(
            ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // 🔹 Single transaction
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransaction(Guid transactionId)
        {
            var userId = User.GetUserId();

            var transaction =
                await _transactionService.GetTransactionAsync(
                    transactionId,
                    userId);

            if (transaction == null)
                return NotFound(new { message = "Transaction not found" });

            return Ok(transaction);
        }

        // 🔹 Logged-in user's history
        [HttpGet("me")]
        public async Task<IActionResult> GetMyTransactions()
        {
            var userId = User.GetUserId();
            return Ok(
                await _transactionService.GetUserTransactionsAsync(userId)
            );
        }

        // 🔹 Wallet-specific history
        [HttpGet("wallet/{walletId}")]
        public async Task<IActionResult> GetWalletTransactions(Guid walletId)
        {
            var userId = User.GetUserId();
            return Ok(
                await _transactionService.GetWalletTransactionsAsync(
                    userId,
                    walletId)
            );
        }
    }
}
