using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using FinEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinEdge.Infrastructure.Repositories;

public class UserRepository(FinEdgeDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Users.FindAsync(id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<int> AddAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await context.Users.AddAsync(user, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);
        
        return result;
    }

    public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await context.Users.AnyAsync(predicate, cancellationToken);
    }
}
