using Microsoft.EntityFrameworkCore;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Infrastructure.Data.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Contents)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<Category?> GetWithContentsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Contents)
                .ThenInclude(content => content.User)
            .Include(c => c.Contents)
                .ThenInclude(content => content.Variants)
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithContentCountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Contents)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Contents)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Contents)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }
} 