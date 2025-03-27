using FinEdge.Domain.Entities;

namespace FinEdge.Application.Common.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(int id);
    Task<Wallet?> GetByUserIdAsync(int userId);
    Task<int> AddAsync(Wallet wallet);
    Task<int> UpdateAsync(Wallet wallet);
}
