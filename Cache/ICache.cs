using System;
using System.Collections.Generic;
using Cache.Objects;


namespace Cache
{

    /// <summary>
    /// Interface for caching operations
    /// </summary>
    public interface ICache
    {
        bool PutItem<T>(T value, int id, params CacheDependency[] dependencies);

        T FetchItem<T>(int id);

        bool PutCollection<T>(IEnumerable<T> collection, Func<T, int> identifierField);

        bool PutAllCollection<T>(IEnumerable<T> collection, Func<T, int> identifierField);

        bool PutCollection<T>(IEnumerable<CacheDependencyItem<T>> items);
        bool PutAllCollection<T>(IEnumerable<CacheDependencyItem<T>> collection);

        IEnumerable<T> FetchCollection<T>(Func<T, int> identifierField, IEnumerable<int> ids, Func<IEnumerable<int>, IEnumerable<T>> callback);

        IEnumerable<T> FetchAllCollection<T>(Func<T, int> identifierField, Func<IEnumerable<int>, IEnumerable<T>> callback);

        bool Remove<T>(T value, int id);
        bool Remove<T>(IEnumerable<T> value, Func<T, int> identifierField);
        //The following two functions are generic string based key insert / fetches. Would rather not use them, but there are cases where they might be useful e.g. caching heavy resource.

        bool PutItem<T>(T value, string key, params CacheDependency[] dependencies);

        T FetchItem<T>(string key);
    }
}
