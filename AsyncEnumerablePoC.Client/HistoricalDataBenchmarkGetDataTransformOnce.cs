using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkGetDataTransformOnce : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetDataTransformOnce")]
    public async Task<double> GetDataCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataC"));

        HistoricalTransformedData[] data = results.Select(d => Transform(Map(d), -7.23)).ToArray();

        double a = 0.0;
        foreach (var result in data)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark, BenchmarkCategory("GetDataTransformOnce")]
    public async Task<double> GetDataAsyncEnum()
    {
        var results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed = MapTransformAsyncEnum(results, -7.23);

        double a = 0.0;
        await foreach (var result in transformed)
        {
            a = result.Value;
        }

        return a;
    }
}
