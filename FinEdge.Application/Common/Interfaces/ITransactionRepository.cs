using FinEdge.Domain.Entities;

namespace FinEdge.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId, CancellationToken cancellationToken);
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
}
