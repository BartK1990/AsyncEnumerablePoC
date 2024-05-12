using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkGetDataTransformTwiceAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetDataTransformTwiceAndSave")]
    public async Task GetDataTransformTwiceAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataC"));

        HistoricalTransformedData[] data1 = results.Select(d => Transform(Map(d), -7)).ToArray();
        HistoricalTransformedData[] data2 = data1.Select(d => Transform(d, 123.2)).ToArray();

        await MongoDataSet.InsertManyAsync(data2);
    }

    [Benchmark, BenchmarkCategory("GetDataTransformTwiceAndSave")]
    public async Task GetDataTransformTwiceAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = MapTransformAsyncEnum(results, -7);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = TransformAsyncEnum(transformed1, 123.2);

        await MongoDataSet.InsertManyAsync(await transformed2.ToArrayAsync());
    }

    [Benchmark, BenchmarkCategory("GetDataTransformTwiceAndSave")]
    public async Task GetDataTransformTwiceAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = MapTransformAsyncEnum(results, -7);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = TransformAsyncEnum(transformed1, 123.2);

        await BatchInsert(transformed2, MongoDataSet);
    }
}
