using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Cache.Objects;
using Extensions;

namespace Cache
{
    public class DevNullCacheProvider : ICache
    {

        private static MemoryCache Cache { get { return MemoryCache.Default; } }

        private static string GenerateCacheName(string type, int id) { return string.Format("{0}_{1}", type, id); }
        private static string GenerateCacheLockName(string name) { return string.Format("LOCK__{0}", name); }


        public bool PutItem<T>(T value, int id, params CacheDependency[] dependencies)
        {
            return true;
        }

        public T FetchItem<T>(int id)
        {
            return default(T);
        }

        public bool PutCollection<T>(IEnumerable<T> collection, Func<T, int> identifierField)
        {
            return true;
        }

        public bool PutAllCollection<T>(IEnumerable<T> collection, Func<T, int> identifierField)
        {
            return true;
        }

        public bool PutCollection<T>(IEnumerable<CacheDependencyItem<T>> items)
        {
            return true;
        }

        public bool PutAllCollection<T>(IEnumerable<CacheDependencyItem<T>> collection)
        {
            return true;
        }

        public bool PutCollection<T>(IEnumerable<CacheDependencyItem<T>> items, IEnumerable<CacheDependencyItem<T>> callback)
        {
            throw new NotImplementedException();
        }

        public bool PutAllCollection<T>(IEnumerable<CacheDependencyItem<T>> collection, IEnumerable<CacheDependencyItem<T>> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> FetchCollection<T>(Func<T, int> identifierField, IEnumerable<int> ids, Func<IEnumerable<int>, IEnumerable<T>> callback)
        {
            return default(IEnumerable<T>);
        }


        public IEnumerable<T> FetchAllCollection<T>(Func<T, int> identifierField, Func<IEnumerable<int>, IEnumerable<T>> callback)
        {
            return default(IEnumerable<T>);
        }

        public bool Remove<T>(T value, int id)
        {
            return true;
        }

        public bool Remove<T>(IEnumerable<T> value, Func<T, int> identifierField)
        {
            return true;
        }

        public bool PutItem<T>(T value, string key, params CacheDependency[] dependencies)
        {
            return true;
        }

        public T FetchItem<T>(string key)
        {
            return default(T);
        }
    }
}
