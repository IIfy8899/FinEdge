using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using FinEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinEdge.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FinEdgeDbContext _context;

    public UserRepository(FinEdgeDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<int> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        var result = await _context.SaveChangesAsync();
        
        return result;
    }

    public async Task<bool> AnyAsync(Expression<Func<User, bool>> predicate)
    {
        return await _context.Users.AnyAsync(predicate);
    }
}
