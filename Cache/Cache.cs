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
    public class Cache
    {
        public class DotNetCache : ICache
        {

            private static MemoryCache Cache { get { return MemoryCache.Default; } }

            private static string GenerateCacheName(string type, int id) { return string.Format("{0}_{1}", type, id); }
            private static string GenerateCacheLockName(string name) { return string.Format("LOCK__{0}", name); }


            public bool PutItem<T>(T value, int id, params CacheDependency[] dependencies)
            {
                if (Cache == null)
                    return false;

                var itemKey = GenerateCacheName(typeof(T).FullName, id);
                var cachePolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) };
                var lockObject = new object();

                Cache.AddOrGetExisting(GenerateCacheLockName(itemKey), lockObject, new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)));

                if (dependencies.HasContent())
                {
                    cachePolicy.ChangeMonitors.Add(Cache.CreateCacheEntryChangeMonitor(dependencies.Select(n => GenerateCacheName(n.Type.FullName, n.ID))));
                }

                lock (lockObject)
                {
                    Cache.Add(itemKey, value, cachePolicy);
                }

                return true;
            }

            private static void OnChangedCallback(object state, bool test)
            {
                var keys = (IEnumerable<string>)state;
                /* if (test)
                 {
                     foreach (var key in keys)
                     {
                         Cache.Remove(key);
                     }
                 }*/
            }


            public T FetchItem<T>(int id)
            {
                if (Cache == null)
                    return default(T);

                var itemKey = GenerateCacheName(typeof(T).FullName, id);

                var cahcedItem = Cache[itemKey];

                if (cahcedItem == null)
                    return default(T);

                return (T)Cache[itemKey];
            }

            public bool PutCollection<T>(IEnumerable<T> collection, Func<T, int> identifierField)
            {
                if (Cache == null)
                    return false;


                if (!collection.HasContent())
                    return false;

                var type = typeof(T).FullName;


                foreach (var item in collection)
                {
                    var itemKey = GenerateCacheName(type, identifierField(item));

                    var lockObject = new object();
                    Cache.AddOrGetExisting(GenerateCacheLockName(itemKey), lockObject, DateTime.Now.AddMinutes(5));

                    lock (lockObject)
                    {
                        Cache.Add(itemKey, item, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) });
                    }
                }


                return true;
            }

            public bool PutAllCollection<T>(IEnumerable<T> collection, Func<T, int> identifierField)
            {
                if (Cache == null)
                    return false;

                if (!collection.HasContent())
                    return false;

                var type = typeof(T).FullName;

                var listKey = string.Format("{0}_ALL", type);

                var ids = collection.Select(identifierField).Distinct();

                var lockObject = new object();

                lock (lockObject)
                {
                    Cache.Add(listKey, ids, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) });
                }

                foreach (var item in collection)
                {
                    var itemKey = GenerateCacheName(type, identifierField(item));

                    Cache.AddOrGetExisting(GenerateCacheLockName(itemKey), lockObject, DateTime.Now.AddMinutes(5));

                    lock (lockObject)
                    {
                        Cache.Add(itemKey, item, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) });
                    }
                }

                return true;
            }

            public bool PutCollection<T>(IEnumerable<CacheDependencyItem<T>> items)
            {
                if (Cache == null)
                    return false;


                if (!items.HasContent())
                    return false;

                var type = typeof(T).FullName;


                foreach (var item in items)
                {
                    var itemKey = GenerateCacheName(type, item.ID);

                    var cachePolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) };
                    var lockObject = new object();

                    Cache.AddOrGetExisting(GenerateCacheLockName(itemKey), lockObject, new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)));

                    if (item.Dependencies.HasContent())
                    {
                        cachePolicy.ChangeMonitors.Add(Cache.CreateCacheEntryChangeMonitor(item.Dependencies.Select(n => GenerateCacheName(n.Type.FullName, n.ID))));
                    }

                    lock (lockObject)
                    {
                        Cache.Add(itemKey, item.Item, cachePolicy);
                    }
                }


                return true;
            }

            public bool PutAllCollection<T>(IEnumerable<CacheDependencyItem<T>> collection)
            {
                if (Cache == null)
                    return false;

                if (!collection.HasContent())
                    return false;

                var type = typeof(T).FullName;

                var listKey = string.Format("{0}_ALL", type);

                var ids = collection.Select(n => n.ID);

                var lockObject = new object();

                lock (lockObject)
                {
                    Cache.Add(listKey, ids, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) });
                }

                foreach (var item in collection)
                {
                    var itemKey = GenerateCacheName(type, item.ID);
                    var cachePolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) };

                    Cache.AddOrGetExisting(GenerateCacheLockName(itemKey), lockObject, DateTime.Now.AddMinutes(5));

                    if (item.Dependencies.HasContent())
                    {
                        cachePolicy.ChangeMonitors.Add(Cache.CreateCacheEntryChangeMonitor(item.Dependencies.Select(n => GenerateCacheName(n.Type.FullName, n.ID))));
                    }

                    lock (lockObject)
                    {
                        Cache.Add(itemKey, item, cachePolicy);
                    }
                }

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
                if (Cache == null)
                    return default(IEnumerable<T>);


                ICollection<T> fetchedItems = new Collection<T>();
                ICollection<int> missingItems = new Collection<int>();
                var type = typeof(T).FullName;

                foreach (var id in ids)
                {
                    var cahcedItem = Cache[GenerateCacheName(type, id)];

                    if (cahcedItem != null)
                        fetchedItems.Add((T)cahcedItem);
                    else
                        missingItems.Add(id);
                }

                if (callback != null && missingItems.HasContent())
                {
                    var nonCachedItems = callback(missingItems);

                    if (missingItems.HasContent())
                    {
                        //cache them!
                        var cachedItems = nonCachedItems as T[] ?? nonCachedItems.ToArray();

                        PutCollection(cachedItems, identifierField);

                        return fetchedItems.Union(cachedItems).ToList();
                    }
                }

                return fetchedItems;
            }


            public IEnumerable<T> FetchAllCollection<T>(Func<T, int> identifierField, Func<IEnumerable<int>, IEnumerable<T>> callback)
            {
                if (Cache == null)
                    return default(ICollection<T>);


                ICollection<T> fetchedItems = new Collection<T>();
                ICollection<int> missingItems = new Collection<int>();
                var type = typeof(T).FullName;

                var ListKey = string.Format("{0}_ALL", type);

                var idsC = Cache[ListKey];

                if (idsC == null)
                    return null;

                var ids = idsC as IEnumerable<int>;

                foreach (var id in ids)
                {
                    var cahcedItem = Cache[GenerateCacheName(type, id)];

                    if (cahcedItem != null)
                        fetchedItems.Add((T)cahcedItem);
                    else
                        missingItems.Add(id);
                }

                if (callback != null && missingItems.HasContent())
                {
                    var nonCachedItems = callback(missingItems);

                    if (missingItems.HasContent())
                    {
                        //cache them!
                        var cachedItems = nonCachedItems as T[] ?? nonCachedItems.ToArray();

                        PutCollection(cachedItems, identifierField);

                        return fetchedItems.Union(cachedItems).ToList();
                    }
                }

                return fetchedItems;
            }

            public bool Remove<T>(T value, int id)
            {
                if (Cache == null)
                    return false;

                var itemKey = GenerateCacheName(typeof(T).FullName, id);

                Cache.Remove(itemKey);

                return true;
            }

            public bool Remove<T>(IEnumerable<T> value, Func<T, int> identifierField)
            {
                if (Cache == null)
                    return false;

                if (!value.HasContent())
                    return false;

                var success = true;
                var typeName = typeof(T).FullName;

                Parallel.ForEach(value, item =>
                {
                    var itemKey = GenerateCacheName(typeName, identifierField(item));

                    Cache.Remove(itemKey);
                });

                /*   foreach (var item in value.AsParallel())
                   {
                       var itemKey = GenerateCacheName(typeName, id);
                       Cache.Remove(itemKey);
                   }*/

                return success;
            }

            public bool PutItem<T>(T value, string key, params CacheDependency[] dependencies)
            {
                if (Cache == null)
                    return false;


                var cachePolicy = new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 20, 0) };
                var lockObject = new object();

                Cache.AddOrGetExisting(GenerateCacheLockName(key), lockObject, new DateTimeOffset(DateTime.UtcNow.AddMinutes(5)));

                if (dependencies.HasContent())
                {
                    cachePolicy.ChangeMonitors.Add(Cache.CreateCacheEntryChangeMonitor(dependencies.Select(n => GenerateCacheName(n.Type.FullName, n.ID))));
                }

                lock (lockObject)
                {
                    Cache.Add(key, value, cachePolicy);
                }

                return true;
            }

            public T FetchItem<T>(string key)
            {
                if (Cache == null)
                    return default(T);

                var cahcedItem = Cache[key];

                if (cahcedItem == null)
                    return default(T);

                return (T)Cache[key];
            }
        }
    }
}
