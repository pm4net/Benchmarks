﻿using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using FSharpx;
using Microsoft.FSharp.Core;
using OCEL.CSharp;
using pm4net.Types;
using OcelLog = OCEL.Types.OcelLog;

namespace Benchmarks
{
    [JsonExporter]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net70, iterationCount: 3, warmupCount: 1)]
    //[SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net481, iterationCount: 3, warmupCount: 1)]
    public class Pm4NetBenchmarks
    {
        [Params("github_pm4py", "o2c", "p2p", "recruiting", "running-example", "transfer_order", "windows_events")]
        public string OcelFile { get; set; }

        private OcelLog _log;

        [IterationSetup]
        public void IterationSetup()
        {
            var json = File.ReadAllText(Path.Combine(@"C:\Users\johan\source\repos\Benchmarks\Benchmarks\Ocel", $"{OcelFile}.jsonocel"));
            _log = OcelJson.Deserialize(json, false).ToFSharpOcelLog();
        }

        [Benchmark]
        public void DiscoverOcDfg()
        {
            pm4net.Algorithms.Discovery.Ocel.OcelDfg.Discover(
                new OcDfgFilter(0, 0, 0, FSharpOption<TimeframeFilter>.None, 
                    new List<LogLevel>
                    {
                        LogLevel.Verbose, 
                        LogLevel.Debug, 
                        LogLevel.Information, 
                        LogLevel.Warning, 
                        LogLevel.Error, 
                        LogLevel.Fatal, 
                        LogLevel.Unknown
                    }.ToFSharpList()), 
                _log.ObjectTypes.ToFSharpList(), 
                _log);
        }
    }
}