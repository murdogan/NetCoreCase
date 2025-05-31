using Microsoft.EntityFrameworkCore;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Infrastructure.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Contents)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<User?> GetWithContentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Contents)
                .ThenInclude(c => c.Category)
            .Include(u => u.Contents)
                .ThenInclude(c => c.Variants)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersWithContentCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Contents)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Contents)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Contents)
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);
    }
} 