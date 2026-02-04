using Microsoft.Extensions.Caching.Memory;
using PIED_LMS.Application.Abstractions;

namespace PIED_LMS.Infrastructure.Authentication;

public class RefreshTokenService(IMemoryCache memoryCache) : IRefreshTokenService
{
    private const string _cacheKeyPrefix = "RefreshToken_";
    private const string _userTokensPrefix = "UserTokens_";
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<string> StoreRefreshTokenAsync(Guid userId, string refreshToken, int expirationDays)
    {
        var cacheKey = $"{_cacheKeyPrefix}{refreshToken}";
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(expirationDays),
            SlidingExpiration = null
        };

        _memoryCache.Set(cacheKey, userId, cacheOptions);
        
        // Track tokens for user to allow RevokeAll
        var userTokensKey = $"{_userTokensPrefix}{userId}";

        // Use Set to ensure options are applied if creating new, or GetOrCreate with options
        // Since GetOrCreate doesn't easily support "apply options only if new" without setup,
        // we use a more explicit approach or GetOrCreate with factory that sets options.
        var userTokens = _memoryCache.GetOrCreate(userTokensKey, entry => 
        {
            entry.Priority = CacheItemPriority.NeverRemove;
            // Align expiration with max possible refresh token lifetime or keep it indefinitely until revoked/cleared
            // Given "NeverRemove" protects against compaction, but not app restart.
            return new HashSet<string>();
        });

        if (userTokens != null)
        {
             lock (userTokens) 
             {
                 userTokens.Add(refreshToken);
             }
        }

        return Task.FromResult(refreshToken);
    }

    public Task<Guid?> GetUserIdFromRefreshTokenAsync(string refreshToken)
    {
        var cacheKey = $"{_cacheKeyPrefix}{refreshToken}";
        if (_memoryCache.TryGetValue<Guid>(cacheKey, out var userId)) return Task.FromResult<Guid?>(userId);

        return Task.FromResult<Guid?>(null);
    }

    public Task<bool> RevokeRefreshTokenAsync(string refreshToken)
    {
        var cacheKey = $"{_cacheKeyPrefix}{refreshToken}";
        var existed = _memoryCache.TryGetValue<Guid>(cacheKey, out var userId);
        if (existed) 
        {
            _memoryCache.Remove(cacheKey);
            
            // Cleanup from user tokens list
            var userTokensKey = $"{_userTokensPrefix}{userId}";
            if (_memoryCache.TryGetValue<HashSet<string>>(userTokensKey, out var userTokens) && userTokens != null)
            {
                lock (userTokens)
                {
                    userTokens.Remove(refreshToken);
                    if (userTokens.Count == 0)
                    {
                        _memoryCache.Remove(userTokensKey);
                    }
                }
            }
        }
        return Task.FromResult(existed);
    }
    
    public Task RevokeAllRefreshTokenAsync(Guid userId)
    {
        var userTokensKey = $"{_userTokensPrefix}{userId}";
        if (_memoryCache.TryGetValue<HashSet<string>>(userTokensKey, out var userTokens) && userTokens != null)
        {
            List<string> tokensToRevoke;
            lock (userTokens)
            {
                tokensToRevoke = userTokens.ToList();
                // Remove tracking entry first while holding lock to prevent new additions to stale set
                _memoryCache.Remove(userTokensKey);
            }

            foreach (var token in tokensToRevoke)
            {
                 var cacheKey = $"{_cacheKeyPrefix}{token}";
                 _memoryCache.Remove(cacheKey);
            }
        }
        
        return Task.CompletedTask;
    }
}
