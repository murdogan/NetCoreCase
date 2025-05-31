using Mapster;
using NetCoreCase.Application.DTOs.User;
using NetCoreCase.Application.Interfaces;
using NetCoreCase.Domain.Entities;
using NetCoreCase.Domain.Interfaces;

namespace NetCoreCase.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private const string CacheKeyPrefix = "users";
    private readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(15);

    public UserService(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:{id}";
        
        // Cache'den kontrol et
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        if (cachedUser != null)
            return cachedUser;

        // Repository'den al
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return null;

        var userDto = user.Adapt<UserDto>();
        userDto.ContentCount = user.Contents?.Count ?? 0;

        // Cache'e kaydet
        await _cacheService.SetAsync(cacheKey, userDto, CacheExpiration, cancellationToken);
        
        return userDto;
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:email:{email}";
        
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        if (cachedUser != null)
            return cachedUser;

        var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
        if (user == null)
            return null;

        var userDto = user.Adapt<UserDto>();
        userDto.ContentCount = user.Contents?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, userDto, CacheExpiration, cancellationToken);
        
        return userDto;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:all";
        
        var cachedUsers = await _cacheService.GetAsync<IEnumerable<UserDto>>(cacheKey, cancellationToken);
        if (cachedUsers != null)
            return cachedUsers;

        var users = await _unitOfWork.Users.GetUsersWithContentCountAsync(cancellationToken);
        var userDtos = users.Select(u =>
        {
            var dto = u.Adapt<UserDto>();
            dto.ContentCount = u.Contents?.Count ?? 0;
            return dto;
        });

        await _cacheService.SetAsync(cacheKey, userDtos, CacheExpiration, cancellationToken);
        
        return userDtos;
    }

    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        // E-posta kontrolü
        if (await _unitOfWork.Users.EmailExistsAsync(createUserDto.Email, cancellationToken))
            throw new InvalidOperationException($"E-posta '{createUserDto.Email}' zaten kullanımda.");

        var user = createUserDto.Adapt<User>();
        user = await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = user.Adapt<UserDto>();
        userDto.ContentCount = 0;

        // Cache'i temizle
        await _cacheService.RemoveByPatternAsync($"{CacheKeyPrefix}:*", cancellationToken);

        return userDto;
    }

    public async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"Kullanıcı bulunamadı: {id}");

        // E-posta başka kullanıcıda var mı kontrol et
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(updateUserDto.Email, cancellationToken);
        if (existingUser != null && existingUser.Id != id)
            throw new InvalidOperationException($"E-posta '{updateUserDto.Email}' başka bir kullanıcı tarafından kullanılıyor.");

        // Güncelle
        user.FullName = updateUserDto.FullName;
        user.Email = updateUserDto.Email;
        user.UpdatedAt = DateTime.UtcNow;

        user = await _unitOfWork.Users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userDto = user.Adapt<UserDto>();
        userDto.ContentCount = user.Contents?.Count ?? 0;

        // Cache'i temizle
        await _cacheService.RemoveByPatternAsync($"{CacheKeyPrefix}:*", cancellationToken);

        return userDto;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
        if (user == null)
            return false;

        // İçerikleri olan kullanıcı silinemez
        var contentCount = await _unitOfWork.Contents.CountAsync(c => c.UserId == id, cancellationToken);
        if (contentCount > 0)
            throw new InvalidOperationException("İçerikleri olan kullanıcı silinemez.");

        await _unitOfWork.Users.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Cache'i temizle
        await _cacheService.RemoveByPatternAsync($"{CacheKeyPrefix}:*", cancellationToken);

        return true;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Users.EmailExistsAsync(email, cancellationToken);
    }

    public async Task<UserDto?> GetWithContentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}:with-contents:{userId}";
        
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey, cancellationToken);
        if (cachedUser != null)
            return cachedUser;

        var user = await _unitOfWork.Users.GetWithContentsAsync(userId, cancellationToken);
        if (user == null)
            return null;

        var userDto = user.Adapt<UserDto>();
        userDto.ContentCount = user.Contents?.Count ?? 0;

        await _cacheService.SetAsync(cacheKey, userDto, CacheExpiration, cancellationToken);
        
        return userDto;
    }
} 