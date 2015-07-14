using System.Collections.Generic;
using TalisScraper.Objects;

namespace TalisScraper.Interfaces
{
    public interface IExportHandler
    {
        string Export(IEnumerable<ReadingList> readingLists);
    }
}
