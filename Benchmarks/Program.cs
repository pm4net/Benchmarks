using System.Reflection;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //var summary = BenchmarkRunner.Run<OcelBenchmarks>(args: args);
            var summary = BenchmarkRunner.Run<SerilogBenchmarks>(args: args);
        }
    }
}