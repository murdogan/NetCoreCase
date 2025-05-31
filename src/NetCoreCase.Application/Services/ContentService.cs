using Mapster;
using Microsoft.Extensions.Logging;
using NetCoreCase.Application.DTOs.Content;
using NetCoreCase.Application.Interfaces;
using NetCoreCase.Domain.Constants;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Application.Services;

public class ContentService : IContentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ContentService> _logger;
    private const string CacheKeyPrefix = "contents";
    private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(15);

    public ContentService(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<ContentService> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:{id}";
        
        var cachedContent = await _cacheService.GetAsync<ContentDto>(cacheKey, cancellationToken);
        if (cachedContent != null)
            return cachedContent;

        var content = await _unitOfWork.Contents.GetWithAllRelationsAsync(id, cancellationToken);
        if (content == null)
            return null;

        var contentDto = content.Adapt<ContentDto>();
        contentDto.VariantCount = content.Variants?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, contentDto, CacheExpiration, cancellationToken);
        
        return contentDto;
    }

    public async Task<IEnumerable<ContentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:all";
        _logger.LogInformation("GetAllAsync çağrıldı. Cache key: {CacheKey}", cacheKey);
        
        var cachedContents = await _cacheService.GetAsync<IEnumerable<ContentDto>>(cacheKey, cancellationToken);
        if (cachedContents != null)
        {
            _logger.LogInformation("Cache'den {Count} içerik döndürülüyor", cachedContents.Count());
            return cachedContents;
        }

        _logger.LogInformation("Cache'de veri yok, veritabanından çekiliyor");
        var contents = await _unitOfWork.Contents.GetContentsWithVariantsAsync(cancellationToken);
        _logger.LogInformation("Veritabanından {Count} içerik alındı", contents.Count());
        
        var contentDtos = contents.Select(c =>
        {
            var dto = c.Adapt<ContentDto>();
            dto.VariantCount = c.Variants?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, contentDtos, CacheExpiration, cancellationToken);
        _logger.LogInformation("Veriler cache'e kaydedildi: {CacheKey}", cacheKey);
        
        return contentDtos;
    }

    public async Task<IEnumerable<ContentDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:user:{userId}";
        
        var cachedContents = await _cacheService.GetAsync<IEnumerable<ContentDto>>(cacheKey, cancellationToken);
        if (cachedContents != null)
            return cachedContents;

        var contents = await _unitOfWork.Contents.GetByUserIdAsync(userId, cancellationToken);
        var contentDtos = contents.Select(c =>
        {
            var dto = c.Adapt<ContentDto>();
            dto.VariantCount = c.Variants?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, contentDtos, CacheExpiration, cancellationToken);
        
        return contentDtos;
    }

    public async Task<IEnumerable<ContentDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:category:{categoryId}";
        
        var cachedContents = await _cacheService.GetAsync<IEnumerable<ContentDto>>(cacheKey, cancellationToken);
        if (cachedContents != null)
            return cachedContents;

        var contents = await _unitOfWork.Contents.GetByCategoryIdAsync(categoryId, cancellationToken);
        var contentDtos = contents.Select(c =>
        {
            var dto = c.Adapt<ContentDto>();
            dto.VariantCount = c.Variants?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, contentDtos, CacheExpiration, cancellationToken);
        
        return contentDtos;
    }

    public async Task<IEnumerable<ContentDto>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default)
    {
        // Dil kontrolü
        if (!LanguageConstants.IsSupported(language))
            throw new ArgumentException($"Desteklenmeyen dil: {language}");

        var cacheKey = $"{CacheKeyPrefix}:language:{language}";
        
        var cachedContents = await _cacheService.GetAsync<IEnumerable<ContentDto>>(cacheKey, cancellationToken);
        if (cachedContents != null)
            return cachedContents;

        var contents = await _unitOfWork.Contents.GetByLanguageAsync(language, cancellationToken);
        var contentDtos = contents.Select(c =>
        {
            var dto = c.Adapt<ContentDto>();
            dto.VariantCount = c.Variants?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, contentDtos, CacheExpiration, cancellationToken);
        
        return contentDtos;
    }

    public async Task<IEnumerable<ContentDto>> GetByCategoryAndLanguageAsync(Guid categoryId, string language, CancellationToken cancellationToken = default)
    {
        // Dil kontrolü
        if (!LanguageConstants.IsSupported(language))
            throw new ArgumentException($"Desteklenmeyen dil: {language}");

        var cacheKey = $"{CacheKeyPrefix}:category:{categoryId}:language:{language}";
        
        var cachedContents = await _cacheService.GetAsync<IEnumerable<ContentDto>>(cacheKey, cancellationToken);
        if (cachedContents != null)
            return cachedContents;

        var contents = await _unitOfWork.Contents.GetByCategoryAndLanguageAsync(categoryId, language, cancellationToken);
        var contentDtos = contents.Select(c =>
        {
            var dto = c.Adapt<ContentDto>();
            dto.VariantCount = c.Variants?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, contentDtos, CacheExpiration, cancellationToken);
        
        return contentDtos;
    }

    public async Task<ContentDto?> GetWithVariantsAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:with-variants:{contentId}";
        
        var cachedContent = await _cacheService.GetAsync<ContentDto>(cacheKey, cancellationToken);
        if (cachedContent != null)
            return cachedContent;

        var content = await _unitOfWork.Contents.GetWithVariantsAsync(contentId, cancellationToken);
        if (content == null)
            return null;

        var contentDto = content.Adapt<ContentDto>();
        contentDto.VariantCount = content.Variants?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, contentDto, CacheExpiration, cancellationToken);
        
        return contentDto;
    }

    public async Task<IEnumerable<ContentDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<ContentDto>();

        var cacheKey = $"{CacheKeyPrefix}:search:{searchTerm}";
        
        var cachedContents = await _cacheService.GetAsync<IEnumerable<ContentDto>>(cacheKey, cancellationToken);
        if (cachedContents != null)
            return cachedContents;

        var contents = await _unitOfWork.Contents.SearchAsync(searchTerm, cancellationToken);
        var contentDtos = contents.Select(c =>
        {
            var dto = c.Adapt<ContentDto>();
            dto.VariantCount = c.Variants?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, contentDtos, TimeSpan.FromMinutes(5), cancellationToken); // Arama sonuçları daha kısa cache'lenir
        
        return contentDtos;
    }

    public async Task<ContentDto> CreateAsync(CreateContentDto createContentDto, CancellationToken cancellationToken = default)
    {
        // Dil kontrolü
        if (!LanguageConstants.IsSupported(createContentDto.Language))
            throw new ArgumentException($"Desteklenmeyen dil: {createContentDto.Language}");

        // Kullanıcı ve kategori kontrolü
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == createContentDto.UserId, cancellationToken);
        if (!userExists)
            throw new InvalidOperationException($"Kullanıcı bulunamadı: {createContentDto.UserId}");

        var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.Id == createContentDto.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new InvalidOperationException($"Kategori bulunamadı: {createContentDto.CategoryId}");

        // En az 2 varyant kontrolü
        if (createContentDto.Variants.Count < 2)
            throw new InvalidOperationException("En az 2 varyant oluşturulmalıdır.");

        // En fazla 1 default varyant olabilir
        var defaultVariantCount = createContentDto.Variants.Count(v => v.IsDefault);
        if (defaultVariantCount > 1)
            throw new InvalidOperationException("En fazla 1 varyant default olarak işaretlenebilir.");

        // Eğer hiçbiri default değilse ilkini default yap
        if (defaultVariantCount == 0)
            createContentDto.Variants.First().IsDefault = true;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Content oluştur
            var content = createContentDto.Adapt<Content>();
            content.Variants.Clear(); // Mapster'dan gelenler temizle
            
            content = await _unitOfWork.Contents.AddAsync(content, cancellationToken);

            // Varyantları oluştur
            foreach (var variantDto in createContentDto.Variants)
            {
                var variant = variantDto.Adapt<ContentVariant>();
                variant.ContentId = content.Id;
                await _unitOfWork.ContentVariants.AddAsync(variant, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // İlişkili verileri yükle
            content = await _unitOfWork.Contents.GetWithAllRelationsAsync(content.Id, cancellationToken);
            
            var contentDto = content!.Adapt<ContentDto>();
            contentDto.VariantCount = content.Variants?.Count ?? 0;

            // Cache'i tamamen temizle
            await _cacheService.ClearAsync(cancellationToken);

            return contentDto;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ContentDto> UpdateAsync(Guid id, UpdateContentDto updateContentDto, CancellationToken cancellationToken = default)
    {
        // Dil kontrolü
        if (!LanguageConstants.IsSupported(updateContentDto.Language))
            throw new ArgumentException($"Desteklenmeyen dil: {updateContentDto.Language}");

        var content = await _unitOfWork.Contents.GetByIdAsync(id, cancellationToken);
        if (content == null)
            throw new InvalidOperationException($"İçerik bulunamadı: {id}");

        // Kategori kontrolü
        var categoryExists = await _unitOfWork.Categories.ExistsAsync(c => c.Id == updateContentDto.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new InvalidOperationException($"Kategori bulunamadı: {updateContentDto.CategoryId}");

        // Güncelle
        content.Title = updateContentDto.Title;
        content.Description = updateContentDto.Description;
        content.Language = updateContentDto.Language;
        content.ImageUrl = updateContentDto.ImageUrl;
        content.CategoryId = updateContentDto.CategoryId;
        content.UpdatedAt = DateTime.UtcNow;

        content = await _unitOfWork.Contents.UpdateAsync(content, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // İlişkili verileri yükle
        content = await _unitOfWork.Contents.GetWithAllRelationsAsync(content.Id, cancellationToken);
        
        var contentDto = content!.Adapt<ContentDto>();
        contentDto.VariantCount = content.Variants?.Count ?? 0;

        // Cache'i tamamen temizle
        await _cacheService.ClearAsync(cancellationToken);

        return contentDto;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var content = await _unitOfWork.Contents.GetByIdAsync(id, cancellationToken);
        if (content == null)
            return false;

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Önce varyantları sil
            var variants = await _unitOfWork.ContentVariants.GetByContentIdAsync(id, cancellationToken);
            foreach (var variant in variants)
            {
                await _unitOfWork.ContentVariants.DeleteAsync(variant, cancellationToken);
            }

            // Sonra content'i sil
            await _unitOfWork.Contents.DeleteAsync(content, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            // Cache'i tamamen temizle
            await _cacheService.ClearAsync(cancellationToken);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ContentVariantDto?> GetUserVariantAsync(Guid contentId, Guid userId, CancellationToken cancellationToken = default)
    {
        var variant = await _unitOfWork.ContentVariants.GetUserVariantAsync(contentId, userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // History kaydı için save
        return variant?.Adapt<ContentVariantDto>();
    }

    public async Task<ContentVariantDto?> GetDefaultVariantAsync(Guid contentId, CancellationToken cancellationToken = default)
    {
        var variant = await _unitOfWork.ContentVariants.GetDefaultVariantAsync(contentId, cancellationToken);
        return variant?.Adapt<ContentVariantDto>();
    }

    public async Task<ContentVariantDto> AddVariantAsync(Guid contentId, CreateContentVariantDto createVariantDto, CancellationToken cancellationToken = default)
    {
        // Content var mı kontrol et
        var contentExists = await _unitOfWork.Contents.ExistsAsync(c => c.Id == contentId, cancellationToken);
        if (!contentExists)
            throw new InvalidOperationException($"İçerik bulunamadı: {contentId}");

        var variant = createVariantDto.Adapt<ContentVariant>();
        variant.ContentId = contentId;

        // Eğer default olarak işaretlendiyse diğer default'ları kaldır
        if (variant.IsDefault)
        {
            await _unitOfWork.ContentVariants.SetDefaultVariantAsync(contentId, variant.Id, cancellationToken);
        }

        variant = await _unitOfWork.ContentVariants.AddAsync(variant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Cache'i tamamen temizle
        await _cacheService.ClearAsync(cancellationToken);

        return variant.Adapt<ContentVariantDto>();
    }

    public async Task<bool> SetDefaultVariantAsync(Guid contentId, Guid variantId, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.ContentVariants.SetDefaultVariantAsync(contentId, variantId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Cache'i tamamen temizle
        await _cacheService.ClearAsync(cancellationToken);

        return true;
    }

    // Stateful varyant yönetimi - Yeni metodlar
    public async Task<ContentDto?> GetContentForUserAsync(Guid contentId, Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:user-content:{userId}:{contentId}";
        
        var cachedContent = await _cacheService.GetAsync<ContentDto>(cacheKey, cancellationToken);
        if (cachedContent != null)
        {
            // Cache'den alınsa bile, kullanıcının erişim zamanını güncelle
            var firstVariantId = cachedContent.Variants?.FirstOrDefault()?.Id ?? Guid.Empty;
            await _unitOfWork.UserContentVariantHistories.CreateOrUpdateHistoryAsync(
                userId, 
                contentId, 
                firstVariantId, 
                cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return cachedContent;
        }

        // İçeriği getir
        var content = await _unitOfWork.Contents.GetWithAllRelationsAsync(contentId, cancellationToken);
        if (content == null)
            return null;

        // Kullanıcıya özel varyantı getir (stateful)
        var userVariant = await _unitOfWork.ContentVariants.GetUserVariantAsync(contentId, userId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken); // History kaydı için save

        var contentDto = content.Adapt<ContentDto>();
        contentDto.VariantCount = content.Variants?.Count ?? 0;
        
        // Kullanıcının göreceği varyantı belirle
        if (userVariant != null)
        {
            contentDto.UserSpecificVariant = userVariant.Adapt<ContentVariantDto>();
        }

        await _cacheService.SetAsync(cacheKey, contentDto, TimeSpan.FromMinutes(5), cancellationToken);
        
        return contentDto;
    }

    public async Task<UserContentVariantHistoryDto?> GetUserVariantHistoryAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default)
    {
        var history = await _unitOfWork.UserContentVariantHistories.GetUserContentHistoryAsync(userId, contentId, cancellationToken);
        if (history == null)
            return null;

        var historyDto = history.Adapt<UserContentVariantHistoryDto>();
        historyDto.UserFullName = history.User?.FullName ?? "";
        historyDto.ContentTitle = history.Content?.Title ?? "";
        historyDto.VariantData = history.Variant?.VariantData ?? "";

        return historyDto;
    }

    public async Task<IEnumerable<UserContentVariantHistoryDto>> GetUserViewHistoryAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var histories = await _unitOfWork.UserContentVariantHistories.GetUserHistoryAsync(userId, cancellationToken);
        
        return histories.Select(h =>
        {
            var dto = h.Adapt<UserContentVariantHistoryDto>();
            dto.UserFullName = h.User?.FullName ?? "";
            dto.ContentTitle = h.Content?.Title ?? "";
            dto.VariantData = h.Variant?.VariantData ?? "";
            return dto;
        });
    }
} 