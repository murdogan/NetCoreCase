using Mapster;
using NetCoreCase.Application.DTOs.Category;
using NetCoreCase.Application.Interfaces;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private const string CacheKeyPrefix = "categories";
    private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30); // Kategoriler daha uzun cache'lenir

    public CategoryService(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:{id}";
        
        var cachedCategory = await _cacheService.GetAsync<CategoryDto>(cacheKey, cancellationToken);
        if (cachedCategory != null)
            return cachedCategory;

        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return null;

        var categoryDto = category.Adapt<CategoryDto>();
        categoryDto.ContentCount = category.Contents?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, categoryDto, CacheExpiration, cancellationToken);
        
        return categoryDto;
    }

    public async Task<CategoryDto?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:name:{name}";
        
        var cachedCategory = await _cacheService.GetAsync<CategoryDto>(cacheKey, cancellationToken);
        if (cachedCategory != null)
            return cachedCategory;

        var category = await _unitOfWork.Categories.GetByNameAsync(name, cancellationToken);
        if (category == null)
            return null;

        var categoryDto = category.Adapt<CategoryDto>();
        categoryDto.ContentCount = category.Contents?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, categoryDto, CacheExpiration, cancellationToken);
        
        return categoryDto;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:all";
        
        var cachedCategories = await _cacheService.GetAsync<IEnumerable<CategoryDto>>(cacheKey, cancellationToken);
        if (cachedCategories != null)
            return cachedCategories;

        var categories = await _unitOfWork.Categories.GetCategoriesWithContentCountAsync(cancellationToken);
        var categoryDtos = categories.Select(c =>
        {
            var dto = c.Adapt<CategoryDto>();
            dto.ContentCount = c.Contents?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, categoryDtos, CacheExpiration, cancellationToken);
        
        return categoryDtos;
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto createCategoryDto, CancellationToken cancellationToken = default)
    {
        // İsim kontrolü
        if (await _unitOfWork.Categories.NameExistsAsync(createCategoryDto.Name, cancellationToken))
            throw new InvalidOperationException($"Kategori adı '{createCategoryDto.Name}' zaten kullanımda.");

        var category = createCategoryDto.Adapt<Category>();
        category = await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var categoryDto = category.Adapt<CategoryDto>();
        categoryDto.ContentCount = 0;

        // Cache'i temizle
        await _cacheService.RemoveByPatternAsync($"{CacheKeyPrefix}:*", cancellationToken);

        return categoryDto;
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto updateCategoryDto, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null)
            throw new InvalidOperationException($"Kategori bulunamadı: {id}");

        // İsim başka kategoride var mı kontrol et
        var existingCategory = await _unitOfWork.Categories.GetByNameAsync(updateCategoryDto.Name, cancellationToken);
        if (existingCategory != null && existingCategory.Id != id)
            throw new InvalidOperationException($"Kategori adı '{updateCategoryDto.Name}' başka bir kategori tarafından kullanılıyor.");

        // Güncelle
        category.Name = updateCategoryDto.Name;
        category.Description = updateCategoryDto.Description;
        category.UpdatedAt = DateTime.UtcNow;

        category = await _unitOfWork.Categories.UpdateAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var categoryDto = category.Adapt<CategoryDto>();
        categoryDto.ContentCount = category.Contents?.Count ?? 0;

        // Cache'i temizle
        await _cacheService.RemoveByPatternAsync($"{CacheKeyPrefix}:*", cancellationToken);

        return categoryDto;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
        if (category == null)
            return false;

        // İçerikleri olan kategori silinemez
        var contentCount = await _unitOfWork.Contents.CountAsync(c => c.CategoryId == id, cancellationToken);
        if (contentCount > 0)
            throw new InvalidOperationException("İçerikleri olan kategori silinemez.");

        await _unitOfWork.Categories.DeleteAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Cache'i temizle
        await _cacheService.RemoveByPatternAsync($"{CacheKeyPrefix}:*", cancellationToken);

        return true;
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Categories.NameExistsAsync(name, cancellationToken);
    }

    public async Task<CategoryDto?> GetWithContentsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:with-contents:{categoryId}";
        
        var cachedCategory = await _cacheService.GetAsync<CategoryDto>(cacheKey, cancellationToken);
        if (cachedCategory != null)
            return cachedCategory;

        var category = await _unitOfWork.Categories.GetWithContentsAsync(categoryId, cancellationToken);
        if (category == null)
            return null;

        var categoryDto = category.Adapt<CategoryDto>();
        categoryDto.ContentCount = category.Contents?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, categoryDto, CacheExpiration, cancellationToken);
        
        return categoryDto;
    }
} 