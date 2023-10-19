using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.IO;

namespace Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
#if DEBUG
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
#else
            var path = Path.GetFullPath(Path.Combine("..", "results", "Benchmarks"));
            Console.WriteLine("Writing results to " + path);
            var customConfig = DefaultConfig.Instance.WithArtifactsPath(path);
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, customConfig);
#endif
        }
    }
}