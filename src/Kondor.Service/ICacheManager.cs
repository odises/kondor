using System;

namespace Kondor.Service
{
    public interface ICacheManager
    {
        T FromCache<T>(string cacheKey, Func<T> actionFunction);
        T FromCache<T>(string cacheKey, Func<T> actionFunction, out bool fromCache);
        void Invalidate(string cacheKey);
        void Clear();
        IDisposable Ignore();
    }
}
