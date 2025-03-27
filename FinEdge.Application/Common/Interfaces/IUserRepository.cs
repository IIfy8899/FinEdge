using FinEdge.Domain.Entities;
using System.Linq.Expressions;

namespace FinEdge.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<int> AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> AnyAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken);
}
