using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using FinEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace FinEdge.Infrastructure.Repositories;

public class TransactionRepository(FinEdgeDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Transactions.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Transactions
            .Where(t => t.WalletId == walletId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await context.Transactions.AddAsync(transaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}
