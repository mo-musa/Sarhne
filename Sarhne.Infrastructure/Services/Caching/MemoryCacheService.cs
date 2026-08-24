using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Sarhne.Application.Contracts.Services.Caching;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Sarhne.Infrastructure.Services.Caching;

public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly ConcurrentDictionary<string, byte> _keys = new();

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out T? value))
        {
            _logger.LogInformation(
                "CACHE HIT | Key: {CacheKey}",
                key);

            return Task.FromResult(value);
        }

        _logger.LogInformation(
            "CACHE MISS | Key: {CacheKey}",
            key);

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration)
    {
        _cache.Set(
            key,
            value,
            expiration);

        _keys.TryAdd(key, 0);

        _logger.LogInformation(
            "CACHE SET | Key: {CacheKey} | Expiration: {Expiration}",
            key,
            expiration);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);

        _keys.TryRemove(key, out _);

        _logger.LogInformation(
            "CACHE REMOVE | Key: {CacheKey}",
            key);


        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix)
    {
        var keys = _keys.Keys
            .Where(x => x.StartsWith(prefix))
            .ToList();

        foreach (var key in keys)
        {
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        _logger.LogInformation(
            "CACHE REMOVE BY PREFIX | Prefix: {Prefix} | RemovedCount: {Count}",
            prefix,
            keys.Count);

        return Task.CompletedTask;
    }
}