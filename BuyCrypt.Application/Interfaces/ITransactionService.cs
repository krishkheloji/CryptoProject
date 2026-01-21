using BuyCrypt.Application.DTOs;

namespace BuyCrypt.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponse?> GetTransactionAsync(
            Guid transactionId,
            Guid userId);

        Task<TransactionHistoryResponse> GetUserTransactionsAsync(
            Guid userId);

        Task<List<TransactionResponse>> GetWalletTransactionsAsync(
            Guid userId,
            Guid walletId);
    }
}
