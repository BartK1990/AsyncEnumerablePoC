using AsyncEnumerablePoC.Client;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance
    .WithOption(ConfigOptions.JoinSummary, true);

BenchmarkRunner.Run(
    typeof(HistoricalDataBenchmark).Assembly,
    config);

//Console.WriteLine("Start ...");
//var testClient = new GetDataAndSave { Samples = 5000 };
//await Task.Delay(2000);
//Console.WriteLine("Feeding ...");
//await ConnectionHelper.FeedData(testClient.HttpClient, testClient.Samples);
//Console.WriteLine("Test 1 ...");
//await testClient.GetDataAndSaveBatchesCollection();
//Console.WriteLine("Test 2 ...");
//await testClient.GetDataAndSaveBatchesAsyncEnum();

//Console.WriteLine("End ...");
//Console.ReadKey();