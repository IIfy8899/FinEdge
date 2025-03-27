using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using FinEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace FinEdge.Infrastructure.Repositories;

public class TransactionRepository(FinEdgeDbContext context) : ITransactionRepository
{
    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await context.Transactions.AddAsync(transaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public IDbContextTransaction BeginTransaction(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return context.Database.BeginTransaction();
    }
}
