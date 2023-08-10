using CsvHelper.Configuration;

namespace Benchmarks
{
    public class FileSize
    {
        public string Format { get; set; }

        public int NoOfEvents { get; set; }

        public string Framework { get; set; }

        public long FileSizeInBytes { get; set; }
    }
}