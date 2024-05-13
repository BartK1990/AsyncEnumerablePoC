using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;

public class HistoricalDataComplexBenchmarkGetDataSqueezeAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetComplexDataSqueezeAndSave")]
    public async Task GetDataTransformThreeAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexTransformedOnceDataC"));

        IEnumerable<HistoricalTransformedData> squeezed = results.Select(d => new HistoricalTransformedData(
            d.Timestamp, 
            d.Value1 + d.Value2 - d.Value3 * d.Value4 + d.Value5));

        await MongoDataSet.InsertManyAsync(squeezed);
    }

    [Benchmark, BenchmarkCategory("GetComplexDataSqueezeAndSave")]
    public async Task GetDataTransformThreeAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexTransformedOnceDataAE"));

        var squeezed = Squeeze(results);

        await MongoDataSet.InsertManyAsync(await squeezed.ToArrayAsync());
    }

    [Benchmark, BenchmarkCategory("GetComplexDataSqueezeAndSave")]
    public async Task GetDataTransformThreeAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexTransformedOnceDataAE"));

        var squeezed = Squeeze(results);

        await BatchInsert(squeezed, MongoDataSet);
    }

    protected static async IAsyncEnumerable<HistoricalTransformedData> Squeeze(IAsyncEnumerable<HistoricalComplexData> dataSets)
    {
        await foreach (var d in dataSets)
        {
            yield return new HistoricalTransformedData(
                d.Timestamp,
                d.Value1 + d.Value2 - d.Value3 * d.Value4 + d.Value5);
        }
    }
}
