using NetCoreCase.Application.DTOs.Content;

namespace NetCoreCase.Application.Interfaces;

public interface IContentService
{
    Task<ContentDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentDto>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentDto>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentDto>> GetByCategoryAndLanguageAsync(Guid categoryId, string language, CancellationToken cancellationToken = default);
    Task<ContentDto?> GetWithVariantsAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContentDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ContentDto> CreateAsync(CreateContentDto createContentDto, CancellationToken cancellationToken = default);
    Task<ContentDto> UpdateAsync(Guid id, UpdateContentDto updateContentDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Varyant yönetimi
    Task<ContentVariantDto?> GetUserVariantAsync(Guid contentId, Guid userId, CancellationToken cancellationToken = default);
    Task<ContentVariantDto?> GetDefaultVariantAsync(Guid contentId, CancellationToken cancellationToken = default);
    Task<ContentVariantDto> AddVariantAsync(Guid contentId, CreateContentVariantDto createVariantDto, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultVariantAsync(Guid contentId, Guid variantId, CancellationToken cancellationToken = default);
    
    // Stateful varyant yönetimi - Yeni metodlar
    Task<ContentDto?> GetContentForUserAsync(Guid contentId, Guid userId, CancellationToken cancellationToken = default);
    Task<UserContentVariantHistoryDto?> GetUserVariantHistoryAsync(Guid userId, Guid contentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserContentVariantHistoryDto>> GetUserViewHistoryAsync(Guid userId, CancellationToken cancellationToken = default);
} 