using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using CsvHelper;
using CsvHelper.Configuration;
using OCEL.Types;
using Serilog;
using Serilog.Enrichers.CallerInfo;
using Serilog.Sinks.OCEL;

namespace Benchmarks
{
    //[RPlotExporter]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net70, iterationCount: 3, warmupCount: 0)]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net481, iterationCount: 3, warmupCount: 0)]
    public class SerilogBenchmarks
    {
        [Params(1, 10, 100, 1_000, 10_000, 100_000)]
        public int NoOfEvents { get; set; }

        [Params("JSON", "XML", "LiteDb")]
        public string Format { get; set; }

        [ParamsAllValues]
        public bool UseEnricher { get; set; }

        private readonly string _projectDir;
        private string _currentFilePath;

        public SerilogBenchmarks()
        {
            //_projectDir = Path.Combine(Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName ?? "\\", "results");
            _projectDir = Path.Combine("C:\\Users\\johan\\source\\repos\\Benchmarks\\Benchmarks", "results");
        }

        [IterationSetup]
        public void IterationSetup()
        {
            var fileGuid = Guid.NewGuid();
            switch (Format)
            {
                case "JSON":
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.OcelJsonSink(new OcelJsonSinkOptions("C:\\Temp", $"log{fileGuid:N}.jsonocel", RollingPeriod.Never, Formatting.None))
                        .Enrich.When(_ => UseEnricher, c => c.WithCallerInfo(true, new List<string> { "Benchmarks" }))
                        .CreateLogger();
                    _currentFilePath = Path.Combine("C:\\Temp", $"log{fileGuid:N}.jsonocel");
                    break;
                case "XML":
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.OcelXmlSink(new OcelXmlSinkOptions("C:\\Temp", $"log{fileGuid:N}.xmlocel", RollingPeriod.Never, Formatting.None))
                        .Enrich.When(_ => UseEnricher, c => c.WithCallerInfo(true, new List<string> { "Benchmarks" }))
                        .CreateLogger();
                    _currentFilePath = Path.Combine("C:\\Temp", $"log{fileGuid:N}.xmlocel");
                    break;
                case "LiteDb":
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.OcelLiteDbSink(new LiteDbSinkOptions("C:\\Temp", $"log{fileGuid:N}.db", RollingPeriod.Never))
                        .Enrich.When(_ => UseEnricher, c => c.WithCallerInfo(true, new List<string> { "Benchmarks" }))
                        .CreateLogger();
                    _currentFilePath = Path.Combine("C:\\Temp", $"log{fileGuid:N}.db");
                    break;
            }
        }
        
        [IterationCleanup]
        public void IterationCleanup()
        {
            var size = File.Exists(_currentFilePath) ? new FileInfo(_currentFilePath)?.Length : 0;
            var fileSize = new FileSize
            {
                Format = Format,
                NoOfEvents = NoOfEvents,
                UseEnricher = UseEnricher,
                Framework = RuntimeInformation.FrameworkDescription,
                FileSizeInBytes = size ?? 0
            };

            Directory.CreateDirectory(_projectDir);
            var csvFile = Path.Combine(_projectDir, "filesizes.csv");
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };

            var firstTimeWriting = !File.Exists(csvFile);
            using (var stream = File.Open(csvFile, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, csvConfig))
            {
                if (firstTimeWriting)
                {
                    csv.WriteHeader<FileSize>();
                    csv.NextRecord(); // https://stackoverflow.com/a/62985779/2102106
                }

                csv.WriteRecord(fileSize);
                csv.NextRecord(); // https://stackoverflow.com/a/62985779/2102106
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
