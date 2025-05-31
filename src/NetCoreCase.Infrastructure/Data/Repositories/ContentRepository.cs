using Microsoft.EntityFrameworkCore;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Infrastructure.Data.Repositories;

public class ContentRepository : BaseRepository<Content>, IContentRepository
{
    public ContentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Content>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Content>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .Where(c => c.CategoryId == categoryId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Content>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .Where(c => c.Language.ToLower() == language.ToLower())
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Content>> GetByCategoryAndLanguageAsync(Guid categoryId, string language, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .Where(c => c.CategoryId == categoryId && c.Language.ToLower() == language.ToLower())
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Content?> GetWithVariantsAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Variants.OrderBy(v => v.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
    }

    public async Task<Content?> GetWithAllRelationsAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants.OrderBy(v => v.CreatedAt))
            .FirstOrDefaultAsync(c => c.Id == contentId, cancellationToken);
    }

    public async Task<IEnumerable<Content>> GetContentsWithVariantsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Content>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedSearchTerm = searchTerm.ToLower().Trim();
        
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .Where(c => 
                c.Title.ToLower().Contains(normalizedSearchTerm) ||
                c.Description.ToLower().Contains(normalizedSearchTerm) ||
                c.Category.Name.ToLower().Contains(normalizedSearchTerm) ||
                c.User.FullName.ToLower().Contains(normalizedSearchTerm))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public override async Task<Content?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<Content>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Category)
            .Include(c => c.Variants)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
} 