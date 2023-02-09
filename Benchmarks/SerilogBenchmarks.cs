using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using OCEL.Types;
using Serilog;
using Serilog.Sinks.OCEL;

namespace Benchmarks
{
    [RPlotExporter]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net70, iterationCount: 5, warmupCount: 3)]
    public class SerilogBenchmarks
    {
        [Params(1, 100, 1_000, 10_000, 100_000)]
        public int NoOfEvents { get; set; }

        [Params("JSON", "XML", "LiteDb")]
        public string Format { get; set; }

        [ParamsAllValues]
        public bool IncludeNamespaceInfo { get; set; }

        [IterationSetup]
        public void IterationSetup()
        {
            switch (Format)
            {
                case "JSON":
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.OcelJsonSink(new OcelJsonSinkOptions("C:\\Temp", $"log{Guid.NewGuid():N}.jsonocel", RollingPeriod.Never, Formatting.None))
                        .CreateLogger();
                    break;
                case "XML":
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.OcelXmlSink(new OcelXmlSinkOptions("C:\\Temp", $"log{Guid.NewGuid():N}.xmlocel", RollingPeriod.Never, Formatting.None))
                        .CreateLogger();
                    break;
                case "LiteDb":
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.OcelLiteDbSink(new LiteDbSinkOptions("C:\\Temp", $"log{Guid.NewGuid():N}.db", RollingPeriod.Never))
                        .CreateLogger();
                    break;
            }
        }

        [Benchmark]
        public int LogMessages()
        {
            for (var i = 0; i < NoOfEvents; i++)
            {
                Log.Information("Log message");
            }

            Log.CloseAndFlush();
            return NoOfEvents;
        }
    }
}
