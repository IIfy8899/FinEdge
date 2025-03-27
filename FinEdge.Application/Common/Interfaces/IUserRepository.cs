using FinEdge.Domain.Entities;
using System.Linq.Expressions;

namespace FinEdge.Application.Common.Interfaces;

public interface IUserRepository
{
    Task<int> AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<bool> AnyAsync(Expression<Func<User, bool>> predicate);
}
