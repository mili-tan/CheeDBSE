using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CheeDBSEngine
{
    class MCache
    {
        public static void Add(string key, object val, int ttl)
        {
            Task.Run(() =>
            {
                if (!MemoryCache.Default.Contains(key))
                    MemoryCache.Default.Add(key, val, DateTimeOffset.Now.AddSeconds(ttl));
            });
        }
        public static void Add(string key, object val, CacheItemPolicy cachePolicy = null)
        {
            Task.Run(() =>
            {
                if (!MemoryCache.Default.Contains(key))
                    MemoryCache.Default.Add(key, val, cachePolicy ?? new CacheItemPolicy());
            });
        }

        public static void Put(string key, object val)
        {
            Task.Run(() =>
            {
                if (!MemoryCache.Default.Contains(key))
                    MemoryCache.Default.Add(key, val, DateTimeOffset.Now.AddMinutes(10));
                else
                    MemoryCache.Default.Set(key, val, DateTimeOffset.Now.AddMinutes(10));
            });
        }

        public static void Del(string key)
        {
            Task.Run(() =>
            {
                if (MemoryCache.Default.Contains(key)) MemoryCache.Default.Remove(key);
            });
        }

        public static bool TryGet(string key, out object val)
        {
            val = string.Empty;
            if (!MemoryCache.Default.Contains(key)) return false;
            val = MemoryCache.Default.Get(key);
            Task.Run(() => MemoryCache.Default.Set(
                key, MemoryCache.Default.Get(key), DateTimeOffset.Now.AddMinutes(10)));
            return true;
        }
    }
}
