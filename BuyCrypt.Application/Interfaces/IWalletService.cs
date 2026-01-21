using BuyCrypt.Application.DTOs;

namespace BuyCrypt.Application.Interfaces
{
    public interface IWalletService
    {
        Task<WalletResponse> GetOrCreateAsync(Guid userId);
        Task<WalletResponse> DepositAsync(Guid userId, decimal amount);
        Task<WalletResponse> WithdrawAsync(Guid userId, decimal amount);
    }
}
