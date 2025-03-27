using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using FinEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinEdge.Infrastructure.Repositories;

public class WalletRepository(FinEdgeDbContext context) : IWalletRepository
{
    public async Task<Wallet?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await context.Wallets.FindAsync(id, cancellationToken);
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);
    }

    public async Task<int> AddAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await context.Wallets.AddAsync(wallet, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task<int> UpdateAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        context.Wallets.Update(wallet);
        var result = await context.SaveChangesAsync(cancellationToken);

        return result;
    }

    public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Database.BeginTransactionAsync(cancellationToken);
    }
}
