using Microsoft.EntityFrameworkCore;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Infrastructure.Data.Repositories;

public class UserContentVariantHistoryRepository : BaseRepository<UserContentVariantHistory>, IUserContentVariantHistoryRepository
{
    public UserContentVariantHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<UserContentVariantHistory?> GetUserContentHistoryAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.User)
            .Include(h => h.Content)
            .Include(h => h.Variant)
            .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId, cancellationToken);
    }

    public async Task<UserContentVariantHistory> CreateOrUpdateHistoryAsync(Guid userId, Guid contentId, Guid variantId, CancellationToken cancellationToken = default)
    {
        var existingHistory = await _dbSet
            .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId, cancellationToken);

        if (existingHistory != null)
        {
            // Mevcut kayıt varsa güncelle
            existingHistory.VariantId = variantId;
            existingHistory.LastAccessedAt = DateTime.UtcNow;
            existingHistory.ViewCount++;
            existingHistory.UpdatedAt = DateTime.UtcNow;
            
            _dbSet.Update(existingHistory);
            return existingHistory;
        }
        else
        {
            // Yeni kayıt oluştur
            var newHistory = new UserContentVariantHistory
            {
                UserId = userId,
                ContentId = contentId,
                VariantId = variantId,
                ViewedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                ViewCount = 1
            };

            await _dbSet.AddAsync(newHistory, cancellationToken);
            return newHistory;
        }
    }

    public async Task<IEnumerable<UserContentVariantHistory>> GetUserHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.Content)
            .Include(h => h.Variant)
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.LastAccessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserContentVariantHistory>> GetContentHistoryAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.User)
            .Include(h => h.Variant)
            .Where(h => h.ContentId == contentId)
            .OrderByDescending(h => h.LastAccessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasUserViewedContentAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(h => h.UserId == userId && h.ContentId == contentId, cancellationToken);
    }

    public override async Task<UserContentVariantHistory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.User)
            .Include(h => h.Content)
            .Include(h => h.Variant)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<UserContentVariantHistory>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.User)
            .Include(h => h.Content)
            .Include(h => h.Variant)
            .OrderByDescending(h => h.LastAccessedAt)
            .ToListAsync(cancellationToken);
    }
} 