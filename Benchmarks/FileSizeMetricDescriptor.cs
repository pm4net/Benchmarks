using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;

namespace Benchmarks
{
    public class FileSizeMetricDescriptor : IMetricDescriptor
    {
        public string Id => "FileSize";
        public string DisplayName => "File Size";
        public string Legend => "File Size";
        public string NumberFormat => "N0";
        public UnitType UnitType => UnitType.Size;
        public string Unit => SizeUnit.KB.Name;
        public bool TheGreaterTheBetter => false;
        public int PriorityInCategory => 0;
        public bool GetIsAvailable(Metric metric) => true;
    }
}