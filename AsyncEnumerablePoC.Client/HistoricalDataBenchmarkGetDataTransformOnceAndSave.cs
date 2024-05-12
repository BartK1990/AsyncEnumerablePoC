using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkGetDataTransformOnceAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetDataTransformOnceAndSave")]
    public async Task GetDataTransformOnceAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataC"));

        HistoricalTransformedData[] data = results.Select(d => Transform(Map(d), -7)).ToArray();

        await MongoDataSet.InsertManyAsync(data);
    }

    [Benchmark, BenchmarkCategory("GetDataTransformOnceAndSave")]
    public async Task GetDataTransformOnceAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed = MapTransformAsyncEnum(results, -7);

        await MongoDataSet.InsertManyAsync(await transformed.ToArrayAsync());
    }

    [Benchmark, BenchmarkCategory("GetDataTransformOnceAndSave")]
    public async Task GetDataTransformOnceAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed = MapTransformAsyncEnum(results, -7);

        await BatchInsert(transformed, MongoDataSet);
    }
}
