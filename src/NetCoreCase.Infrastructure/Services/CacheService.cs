using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NetCoreCase.Application.Interfaces;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NetCoreCase.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheService> _logger;
    private readonly HashSet<string> _cacheKeys;
    private readonly object _lock = new();

    public CacheService(IMemoryCache memoryCache, ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
        _cacheKeys = new HashSet<string>();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        if (_memoryCache.TryGetValue(key, out var value))
        {
            if (value is string jsonString)
            {
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            return (T?)value;
        }
        
        return await Task.FromResult<T?>(null);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        var options = new MemoryCacheEntryOptions();
        
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration.Value;
        }
        else
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30); // Default 30 minutes
        }

        // Remove callback to clean up our key tracking
        options.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
        {
            EvictionCallback = (key, value, reason, state) =>
            {
                lock (_lock)
                {
                    _cacheKeys.Remove(key.ToString()!);
                }
            }
        });

        // Serialize to JSON for consistent storage
        var jsonString = JsonSerializer.Serialize(value);
        
        _memoryCache.Set(key, jsonString, options);
        
        lock (_lock)
        {
            _cacheKeys.Add(key);
        }

        await Task.CompletedTask;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);
        
        lock (_lock)
        {
            _cacheKeys.Remove(key);
        }

        await Task.CompletedTask;
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cache temizleniyor. Pattern: {Pattern}", pattern);
        
        // Convert glob pattern to regex
        var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
        var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
        
        List<string> keysToRemove;
        
        lock (_lock)
        {
            keysToRemove = _cacheKeys.Where(key => regex.IsMatch(key)).ToList();
        }

        _logger.LogInformation("Temizlenecek cache key sayısı: {Count}. Keys: {Keys}", 
            keysToRemove.Count, string.Join(", ", keysToRemove));

        foreach (var key in keysToRemove)
        {
            await RemoveAsync(key, cancellationToken);
        }
        
        _logger.LogInformation("Cache temizleme tamamlandı.");
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var exists = _memoryCache.TryGetValue(key, out _);
        return await Task.FromResult(exists);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        List<string> allKeys;
        
        lock (_lock)
        {
            allKeys = _cacheKeys.ToList();
        }

        foreach (var key in allKeys)
        {
            await RemoveAsync(key, cancellationToken);
        }
    }
} 