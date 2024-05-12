using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkGetData : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetData")]
    public async Task<double> GetDataCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataC"));

        double a = 0.0;
        foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark, BenchmarkCategory("GetData")]
    public async Task<double> GetDataAsyncEnum()
    {
        var results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"));

        double a = 0.0;
        await foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }
}
