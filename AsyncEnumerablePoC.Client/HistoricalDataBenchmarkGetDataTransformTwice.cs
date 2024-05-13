using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkGetDataTransformTwice : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetDataTransformTwice")]
    public async Task<double> GetDataCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataC"));

        HistoricalTransformedData[] data1 = results.Select(d => Transform(Map(d), -7.23)).ToArray();
        HistoricalTransformedData[] data2 = data1.Select(d => Transform(d, 123.2)).ToArray();

        double a = 0.0;
        foreach (var result in data2)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark, BenchmarkCategory("GetDataTransformTwice")]
    public async Task<double> GetDataAsyncEnum()
    {
        var results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = MapTransformAsyncEnum(results, -7.23);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = TransformAsyncEnum(transformed1, 123.2);

        double a = 0.0;
        await foreach (var result in transformed2)
        {
            a = result.Value;
        }

        return a;
    }
}
