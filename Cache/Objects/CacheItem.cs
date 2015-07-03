using System.Collections.Generic;

namespace Cache.Objects
{
    public class CacheItem<T>
    {
        public T Item { get; set; }

        public IEnumerable<string> Related { get; set; }
    }
}
