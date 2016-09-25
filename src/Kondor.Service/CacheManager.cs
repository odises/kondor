using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace Kondor.Service
{
    public class CacheManager : ICacheManager
    {
        protected const string IgnoreCacheScopeName = "ignorecache";

        private ObjectCache Cache => MemoryCache.Default;
        private static readonly List<string> Keys = new List<string>();

        public virtual T FromCache<T>(string cacheKey, Func<T> actionFunction)
        {
            bool temp;
            return FromCache(cacheKey, actionFunction, out temp);
        }

        public virtual T FromCache<T>(string cacheKey, Func<T> actionFunction, out bool fromCache)
        {
            fromCache = false;

            if (ThreadScope.IsInScope(IgnoreCacheScopeName))
            {
                return actionFunction();
            }
            var item = Cache.Get(cacheKey);
            if (item == null)
            {
                var value = actionFunction();
                Cache.Set(cacheKey, value, new CacheItemPolicy());

                Keys.Add(cacheKey);

                return value;
            }

            fromCache = true;
            return (T)item;
        }

        public void Invalidate(string cacheKey)
        {
            Cache.Remove(cacheKey);
            Keys.Remove(cacheKey);
        }

        public void Clear()
        {
            foreach (var key in Keys)
            {
                Cache.Remove(key);
            }
        }

        public IDisposable Ignore()
        {
            return new ThreadScope(IgnoreCacheScopeName);
        }
    }
}
