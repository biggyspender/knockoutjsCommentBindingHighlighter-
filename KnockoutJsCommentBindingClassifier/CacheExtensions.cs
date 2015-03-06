using System;
using System.Runtime.Caching;
using System.Web.Caching;

namespace KnockoutJsCommentBindingClassifier
{
    public static class CacheExtensions
    {
        public const int DefaultCacheExpiration = 20;
        private static readonly object sync = new object();

        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        /// <example><![CDATA[
        /// var user = HttpRuntime
        ///   .Cache
        ///   .GetOrStore<User>(
        ///      string.Format("User{0}", _userId), 
        ///      () => Repository.GetUser(_userId));
        ///
        /// ]]></example>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">calling object</param>
        /// <param name="key">Cache key</param>
        /// <param name="generator">Func that returns the object to store in cache</param>
        /// <returns></returns>
        /// <remarks>Uses a default cache expiration period as defined in <see cref="CacheExtensions.DefaultCacheExpiration"/></remarks>
        private static T GetOrStore<T>(this ICache cache, string key, Func<T> generator)
        {
            return cache.GetOrStore(key, (cache[key] == null && generator != null) ? generator() : default(T),
                DefaultCacheExpiration);
        }

        public static T GetOrStore<T>(this Cache cache, string key, Func<T> generator)
        {
            return cache.ToICache().GetOrStore(key, generator);
        }

        public static T GetOrStore<T>(this MemoryCache cache, string key, Func<T> generator)
        {
            return cache.ToICache().GetOrStore(key, generator);
        }

        private static ICache ToICache(this Cache cache)
        {
            return new HttpCacheWrapper(cache);
        }

        private static ICache ToICache(this MemoryCache cache)
        {
            return new MemoryCacheWrapper(cache);
        }

        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        /// <example><![CDATA[
        /// var user = HttpRuntime
        ///   .Cache
        ///   .GetOrStore<User>(
        ///      string.Format("User{0}", _userId), 
        ///      () => Repository.GetUser(_userId));
        ///
        /// ]]></example>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">calling object</param>
        /// <param name="key">Cache key</param>
        /// <param name="generator">Func that returns the object to store in cache</param>
        /// <param name="expireInMinutes">Time to expire cache in minutes</param>
        /// <returns></returns>
        public static T GetOrStore<T>(this MemoryCache cache, string key, Func<T> generator, double expireInMinutes)
        {
            return cache.ToICache().GetOrStore(key, generator, expireInMinutes);
        }

        public static T GetOrStore<T>(this Cache cache, string key, Func<T> generator, double expireInMinutes)
        {
            return cache.ToICache().GetOrStore(key, generator, expireInMinutes);
        }

        private static T GetOrStore<T>(this ICache cache, string key, Func<T> generator, double expireInMinutes)
        {
            return cache.GetOrStore(key, (cache[key] == null && generator != null) ? generator() : default(T),
                expireInMinutes);
        }

        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        /// <example><![CDATA[
        /// var user = HttpRuntime
        ///   .Cache
        ///   .GetOrStore<User>(
        ///      string.Format("User{0}", _userId),_userId));
        ///
        /// ]]></example>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">calling object</param>
        /// <param name="key">Cache key</param>
        /// <param name="obj">Object to store in cache</param>
        /// <returns></returns>
        /// <remarks>Uses a default cache expiration period as defined in <see cref="CacheExtensions.DefaultCacheExpiration"/></remarks>
        public static T GetOrStore<T>(this Cache cache, string key, T obj)
        {
            return cache.ToICache().GetOrStore<T>(key, obj);
        }

        public static T GetOrStore<T>(this MemoryCache cache, string key, T obj)
        {
            return cache.ToICache().GetOrStore<T>(key, obj);
        }

        private static T GetOrStore<T>(this ICache cache, string key, T obj)
        {
            return cache.GetOrStore(key, obj, DefaultCacheExpiration);
        }

        /// <summary>
        /// Allows Caching of typed data
        /// </summary>
        /// <example><![CDATA[
        /// var user = HttpRuntime
        ///   .Cache
        ///   .GetOrStore<User>(
        ///      string.Format("User{0}", _userId), 
        ///      () => Repository.GetUser(_userId));
        ///
        /// ]]></example>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache">calling object</param>
        /// <param name="key">Cache key</param>
        /// <param name="obj">Object to store in cache</param>
        /// <param name="expireInMinutes">Time to expire cache in minutes</param>
        /// <returns></returns>
        public static T GetOrStore<T>(this Cache cache, string key, T obj, double expireInMinutes)
        {
            return cache.ToICache().GetOrStore<T>(key, obj, expireInMinutes);
        }

        public static T GetOrStore<T>(this MemoryCache cache, string key, T obj, double expireInMinutes)
        {
            return cache.ToICache().GetOrStore<T>(key, obj, expireInMinutes);
        }

        private static T GetOrStore<T>(this ICache cache, string key, T obj, double expireInMinutes)
        {
            var result = cache[key];

            if (result == null)
            {
                lock (sync)
                {
                    if (result == null)
                    {
                        result = obj != null ? obj : default(T);
                        cache.Insert(key, result, DateTime.Now.AddMinutes(expireInMinutes));
                    }
                }
            }

            return (T) result;
        }

        private class HttpCacheWrapper : ICache
        {
            private readonly Cache cache;

            public HttpCacheWrapper(Cache cache)
            {
                this.cache = cache;
            }

            public void Insert(string key, object value, DateTime expiryTime)
            {
                cache.Insert(key, value, null, expiryTime, Cache.NoSlidingExpiration);
            }

            public object this[string key]
            {
                get { return cache[key]; }
            }
        }

        private interface ICache
        {
            object this[string key] { get; }
            void Insert(string key, object value, DateTime expiryTime);
        }

        private class MemoryCacheWrapper : ICache
        {
            private readonly MemoryCache cache;

            public MemoryCacheWrapper(MemoryCache cache)
            {
                this.cache = cache;
            }

            public void Insert(string key, object value, DateTime expiryTime)
            {
                cache.Add(key, value, expiryTime);
            }

            public object this[string key]
            {
                get { return cache[key]; }
            }
        }
    }
}