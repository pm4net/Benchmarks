using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using LiteDB;
using OCEL.CSharp;
using OCEL.Types;
using OcelEvent = OCEL.CSharp.OcelEvent;
using OcelLog = OCEL.CSharp.OcelLog;
using OcelObject = OCEL.CSharp.OcelObject;
using OcelValue = OCEL.CSharp.OcelValue;

namespace Benchmarks
{
    [JsonExporter]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net70, iterationCount: 5, warmupCount: 3)]
    [SimpleJob(RunStrategy.Monitoring, RuntimeMoniker.Net481, iterationCount: 5, warmupCount: 3)]
    public class OcelBenchmarks
    {
        [Params(1, 10, 100, 1_000, 10_000, 100_000)]
        public int NoOfEvents { get; set; }

        [Params("JSON", "XML", "LiteDb")]
        public string Format { get; set; }

        private OcelLog _log;
        private string _json;
        private string _xml;
        private LiteDatabase _db;

        [IterationSetup]
        public void IterationSetup()
        {
            // Generate OCEL log object with number of events defined by parameter
            var @event = new OcelEvent("Log message", DateTimeOffset.Now, new List<string>(), new Dictionary<string, OcelValue>());
            var dict = new Dictionary<string, OcelEvent>();
            for (var i = 0; i < NoOfEvents; i++)
            {
                dict.Add(Guid.NewGuid().ToString(), @event);
            }
            _log = new OcelLog(new Dictionary<string, OcelValue>(), dict, new Dictionary<string, OcelObject>());

            // Serialize OCEL log to JSON, XML, and LiteDb so that it can be used to benchmark deserialization and validation
            _json = OcelJson.Serialize(_log, Formatting.None, false);
            _xml = OcelXml.Serialize(_log, Formatting.None, false);
            _db = new LiteDatabase(":memory:");
            OcelLiteDB.Serialize(_db, _log, false);
        }

        [Benchmark]
        public string Serialize()
        {
            switch (Format)
            {
                case "JSON":
                    return OcelJson.Serialize(_log, Formatting.None, false);
                case "XML":
                    return OcelXml.Serialize(_log, Formatting.None, false);
                case "LiteDb":
                    // Can also use :temp: to write to actual file instead of memory, but doesn't make a significant difference.
                    var db = new LiteDatabase(":memory:");
                    OcelLiteDB.Serialize(db, _log, false);
                    db.Dispose();
                    break;
            }

            return string.Empty;
        }

        [Benchmark]
        public OcelLog Deserialize()
        {
            switch (Format)
            {
                case "JSON":
                    return OcelJson.Deserialize(_json, false);
                case "XML":
                    return OcelXml.Deserialize(_xml, false);
                case "LiteDb":
                    var log = OcelLiteDB.Deserialize(_db);
                    _db.Dispose();
                    return log;  
            }

            return null;
        }
    }
}