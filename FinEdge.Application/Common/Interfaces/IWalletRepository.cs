using FinEdge.Domain.Entities;

namespace FinEdge.Application.Common.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<int> AddAsync(Wallet wallet, CancellationToken cancellationToken);
    Task<int> UpdateAsync(Wallet wallet, CancellationToken cancellationToken);
}
