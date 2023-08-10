using CsvHelper.Configuration;

namespace Benchmarks
{
    public sealed class FileSizeMap : ClassMap<FileSize>
    {
        public FileSizeMap()
        {
            Map(m => m.Format).Index(0).Name("Format");
            Map(m => m.NoOfEvents).Index(1).Name("NoOfEvents");
            Map(m => m.Framework).Index(2).Name("Framework");
            Map(m => m.FileSizeInBytes).Index(3).Name("FileSizeInBytes");
        }
    }
}