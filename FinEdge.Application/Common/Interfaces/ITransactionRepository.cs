using FinEdge.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinEdge.Application.Common.Interfaces;

public interface ITransactionRepository
{
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken);
    IDbContextTransaction BeginTransaction(CancellationToken cancellationToken);
}
