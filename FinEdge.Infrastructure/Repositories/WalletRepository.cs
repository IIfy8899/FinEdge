using FinEdge.Application.Common.Interfaces;
using FinEdge.Domain.Entities;
using FinEdge.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinEdge.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly FinEdgeDbContext _context;

    public WalletRepository(FinEdgeDbContext context)
    {
        _context = context;
    }

    public async Task<Wallet?> GetByIdAsync(int id)
    {
        return await _context.Wallets.FindAsync(id);
    }

    public async Task<Wallet?> GetByUserIdAsync(int userId)
    {
        return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
    }

    public async Task<int> AddAsync(Wallet wallet)
    {
        await _context.Wallets.AddAsync(wallet);
        var result = await _context.SaveChangesAsync();

        return result;
    }

    public async Task<int> UpdateAsync(Wallet wallet)
    {
        _context.Wallets.Update(wallet);
        var result = await _context.SaveChangesAsync();

        return result;
    }
}
