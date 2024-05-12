using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;
using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client;

public class HistoricalDataComplexBenchmark : HistoricalDataBenchmark
{
    public new IMongoCollection<HistoricalTransformedComplexData> MongoDataSet =>
        SaveDataDbContext.DbSet.HistoricalTransformedComplexDataSets;

    [Benchmark(Baseline = true), BenchmarkCategory("GetComplexDataTransformThreeAndSave")]
    public async Task GetDataTransformThreeAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexTransformedOnceDataC"));

        HistoricalTransformedComplexData[] data1 = results.Select(d => Transform(Map(d), -7)).ToArray();
        HistoricalTransformedComplexData[] data2 = data1.Select(d => Transform(d, 3)).ToArray();
        HistoricalTransformedComplexData[] data3 = data2.Select(d => Transform(d, 4)).ToArray();

        await MongoDataSet.InsertManyAsync(data3);
    }

    [Benchmark, BenchmarkCategory("GetComplexDataTransformThreeAndSave")]
    public async Task GetDataTransformThreeAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedComplexData> transformed1 = MapTransform(results, -7);
        IAsyncEnumerable<HistoricalTransformedComplexData> transformed2 = Transform(transformed1, 123.2);
        IAsyncEnumerable<HistoricalTransformedComplexData> transformed3 = Transform(transformed2, 4);

        await MongoDataSet.InsertManyAsync(await transformed3.ToArrayAsync());
    }

    [Benchmark, BenchmarkCategory("GetComplexDataTransformThreeAndSave")]
    public async Task GetDataTransformThreeAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedComplexData> transformed1 = MapTransform(results, -7);
        IAsyncEnumerable<HistoricalTransformedComplexData> transformed2 = Transform(transformed1, 123.2);
        IAsyncEnumerable<HistoricalTransformedComplexData> transformed3 = Transform(transformed2, 4);

        await BatchInsert(transformed3, MongoDataSet);
    }

    private static async IAsyncEnumerable<HistoricalTransformedComplexData> MapTransform(IAsyncEnumerable<HistoricalComplexData> dataSets, double val)
    {
        await foreach (var data in dataSets)
        {
            yield return Transform(Map(data), val);
        }
    }

    private static async IAsyncEnumerable<HistoricalTransformedComplexData> Transform(IAsyncEnumerable<HistoricalTransformedComplexData> dataSets, double val)
    {
        await foreach (var data in dataSets)
        {
            yield return Transform(data, val);
        }
    }

    private static HistoricalTransformedComplexData Map(HistoricalComplexData data)
    {
        return new HistoricalTransformedComplexData(
            data.Timestamp,
            data.Value1,
            data.Value2,
            data.Value3,
            data.Value4,
            data.Value5);
    }

    private static HistoricalTransformedComplexData Transform(HistoricalTransformedComplexData data, double val)
    {
        return data with
        {
            Value1 =+ val,
            Value2 =+ val,
            Value3 =+ val,
            Value4 =+ val,
            Value5 =+ val,
        };
    }

    protected override async Task ClearMongo()
    {
        await SaveDataDbContext.DbSet.HistoricalTransformedComplexDataSets.DeleteManyAsync(
            Builders<HistoricalTransformedComplexData>.Filter.Empty); // Delete all
    }
}
