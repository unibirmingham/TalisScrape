using System.Collections.Generic;

namespace Cache.Objects
{
    public class CacheDependencyItem<T>
    {
        public int ID { get; set; }
        public T Item { get; set; }
        public IEnumerable<CacheDependency> Dependencies { get; set; }
    }
}
