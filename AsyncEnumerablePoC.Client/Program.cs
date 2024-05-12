using AsyncEnumerablePoC.Client;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance
    .WithOption(ConfigOptions.JoinSummary, true);

BenchmarkRunner.Run(
    typeof(HistoricalDataBenchmark).Assembly,
    config);

//BenchmarkRunner.Run<HistoricalDataBenchmarkDbReadOnly>();
//BenchmarkRunner.Run<HistoricalDataBenchmarkDbReadTransformedTwice>();
//BenchmarkRunner.Run<HistoricalDataBenchmarkGetData>();
//BenchmarkRunner.Run<HistoricalDataBenchmarkGetDataTransformOnceAndSave>();
//BenchmarkRunner.Run<HistoricalDataBenchmarkGetDataTransformTwiceAndSave>();
//BenchmarkRunner.Run<HistoricalDataBenchmarkGetDataTransformThreeAndSave>();
//BenchmarkRunner.Run<HistoricalDataComplexBenchmark>();