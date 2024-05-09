using AsyncEnumerablePoC.Client;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<DataFlowBenchmark>();

//Console.WriteLine("Start");
//await Task.Delay(5000);

//var client = new DataFlowBenchmark();
//await client.GetDataTransformOnceAndSaveCollection();

//Console.WriteLine("End");
