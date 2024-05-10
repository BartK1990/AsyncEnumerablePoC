using AsyncEnumerablePoC.Client;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance
    .WithOption(ConfigOptions.JoinSummary, true);

BenchmarkRunner.Run<HistoricalDataBenchmarkDbReadOnly>(config);
BenchmarkRunner.Run<HistoricalDataBenchmarkDbReadTransformedTwice>(config);
BenchmarkRunner.Run<HistoricalDataBenchmarkGetData>(config);
BenchmarkRunner.Run<HistoricalDataBenchmarkGetDataTransformOnceAndSave>(config);
BenchmarkRunner.Run<HistoricalDataBenchmarkGetDataTransformTwiceAndSave>(config);
BenchmarkRunner.Run<HistoricalDataBenchmarkGetDataTransformThreeAndSave>(config);
BenchmarkRunner.Run<HistoricalDataComplexBenchmark>(config);