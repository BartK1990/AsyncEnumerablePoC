using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;
public class HistoricalDataBenchmarkGetDataTransformThreeAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true)]
    public async Task GetDataTransformThreeAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataC"));

        HistoricalTransformedData[] data1 = results.Select(d => Transform(Map(d), -7)).ToArray();
        HistoricalTransformedData[] data2 = data1.Select(d => Transform(d, 123.2)).ToArray();
        HistoricalTransformedData[] data3 = data2.Select(d => Transform(d, -321.5)).ToArray();

        await MongoDataSet.InsertManyAsync(data3);
    }

    [Benchmark]
    public async Task GetDataTransformThreeAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = MapTransformAsyncEnum(results, -7);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = TransformAsyncEnum(transformed1, 123.2);
        IAsyncEnumerable<HistoricalTransformedData> transformed3 = TransformAsyncEnum(transformed2, -321.5);

        await MongoDataSet.InsertManyAsync(await transformed3.ToArrayAsync());
    }

    [Benchmark]
    public async Task GetDataTransformThreeAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = MapTransformAsyncEnum(results, -7);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = TransformAsyncEnum(transformed1, 123.2);
        IAsyncEnumerable<HistoricalTransformedData> transformed3 = TransformAsyncEnum(transformed2, -321.5);

        await BatchInsert(transformed3, MongoDataSet);
    }
}
