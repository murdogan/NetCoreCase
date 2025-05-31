using Microsoft.EntityFrameworkCore;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Infrastructure.Data.Repositories;

public class ContentVariantRepository : BaseRepository<ContentVariant>, IContentVariantRepository
{
    public ContentVariantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ContentVariant>> GetByContentIdAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cv => cv.Content)
            .Where(cv => cv.ContentId == contentId)
            .OrderBy(cv => cv.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ContentVariant?> GetDefaultVariantAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cv => cv.Content)
            .FirstOrDefaultAsync(cv => cv.ContentId == contentId && cv.IsDefault, cancellationToken);
    }

    public async Task<ContentVariant?> GetUserVariantAsync(Guid contentId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Önce kullanıcının daha önce gördüğü varyantı kontrol et
        var userHistory = await _context.Set<UserContentVariantHistory>()
            .Include(h => h.Variant)
            .FirstOrDefaultAsync(h => h.UserId == userId && h.ContentId == contentId, cancellationToken);

        if (userHistory != null)
        {
            // Kullanıcı daha önce bu içeriği görmüş, aynı varyantı döndür
            // Erişim zamanını güncelle
            userHistory.LastAccessedAt = DateTime.UtcNow;
            userHistory.ViewCount++;
            userHistory.UpdatedAt = DateTime.UtcNow;
            _context.Set<UserContentVariantHistory>().Update(userHistory);
            
            return userHistory.Variant;
        }

        // İlk kez görüyorsa, default varyantı döndür
        var defaultVariant = await GetDefaultVariantAsync(contentId, cancellationToken);
        if (defaultVariant != null)
        {
            // Kullanıcının bu varyantı gördüğünü kaydet
            var history = new UserContentVariantHistory
            {
                UserId = userId,
                ContentId = contentId,
                VariantId = defaultVariant.Id,
                ViewedAt = DateTime.UtcNow,
                LastAccessedAt = DateTime.UtcNow,
                ViewCount = 1
            };
            
            await _context.Set<UserContentVariantHistory>().AddAsync(history, cancellationToken);
        }

        return defaultVariant;
    }

    public async Task SetDefaultVariantAsync(Guid contentId, Guid variantId, CancellationToken cancellationToken = default)
    {
        // Transaction içinde çalışacak, Unit of Work tarafından yönetilecek
        
        // Önce bu content'in tüm varyantlarını default olmaktan çıkar
        var existingVariants = await _dbSet
            .Where(cv => cv.ContentId == contentId)
            .ToListAsync(cancellationToken);

        foreach (var variant in existingVariants)
        {
            variant.IsDefault = false;
            variant.UpdatedAt = DateTime.UtcNow;
        }

        // Yeni default varyantı belirle
        var newDefaultVariant = await _dbSet
            .FirstOrDefaultAsync(cv => cv.Id == variantId && cv.ContentId == contentId, cancellationToken);

        if (newDefaultVariant != null)
        {
            newDefaultVariant.IsDefault = true;
            newDefaultVariant.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<bool> ContentHasVariantsAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(cv => cv.ContentId == contentId, cancellationToken);
    }

    public override async Task<ContentVariant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cv => cv.Content)
            .FirstOrDefaultAsync(cv => cv.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<ContentVariant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cv => cv.Content)
            .OrderBy(cv => cv.ContentId)
            .ThenBy(cv => cv.CreatedAt)
            .ToListAsync(cancellationToken);
    }
} 