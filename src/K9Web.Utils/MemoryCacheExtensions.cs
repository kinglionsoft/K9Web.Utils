using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Microsoft.Extensions.Caching.Memory
{
    public static class MemoryCacheExtensions
    {
        private static CancellationTokenSource _resetCacheToken = new CancellationTokenSource();

        public static TItem GetOrCreateWithCancellationToken<TItem>(this IMemoryCache cache,
            object key,
            Func<ICacheEntry, TItem> factory)
        {
            return cache.GetOrCreate(key, entry =>
            {
                entry.ExpirationTokens.Add(new CancellationChangeToken(_resetCacheToken.Token));
                return factory(entry);

            });
        }

        public static Task<TItem> GetOrCreateWithCancellationTokenAsync<TItem>(this IMemoryCache cache,
            object key,
            Func<ICacheEntry, Task<TItem>> factory)
        {
            return cache.GetOrCreateAsync(key, entry =>
            {
                entry.ExpirationTokens.Add(new CancellationChangeToken(_resetCacheToken.Token));
                return factory(entry);

            });
        }

        public static void Reset(this IMemoryCache cache)
        {
            if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
            {
                _resetCacheToken.Cancel();
                _resetCacheToken.Dispose();
            }

            _resetCacheToken = new CancellationTokenSource();
        }
    }
}
