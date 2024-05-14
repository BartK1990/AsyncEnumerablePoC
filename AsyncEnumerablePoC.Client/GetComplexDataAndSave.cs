using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;
using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client;

public class GetComplexDataAndSave : HistoricalDataBenchmark
{
    public new IMongoCollection<HistoricalTransformedComplexData> MongoDataSet =>
        SaveDataDbContext.DbSet.HistoricalTransformedComplexDataSets;

    [Benchmark(Baseline = true), BenchmarkCategory("GetComplexDataMapAndSave")]
    public async Task GetDataTransformThreeAndSaveBatchesCollection()
    {
        int count = Samples / BatchSize;
        for (int i = 0; i < count; i++)
        {
            IReadOnlyCollection<HistoricalComplexData> results = await CollectionReceiver.PostData<HistoricalComplexData>(
                HttpClient,
                Url.Combine("HistoricalData", "GetComplexDataC-Batch"),
                new GetDataBatchRequest { BatchCount = i, BatchSize = BatchSize });

            await MongoDataSet.InsertManyAsync(results.Select(Map));
        }
    }

    [Benchmark, BenchmarkCategory("GetComplexDataMapAndSave")]
    public async Task GetDataTransformThreeAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexDataAE"));

        var mapped = MapAsync(results);

        await BatchInsert(mapped, MongoDataSet);
    }

    protected static async IAsyncEnumerable<HistoricalTransformedComplexData> MapAsync(IAsyncEnumerable<HistoricalComplexData> dataSets)
    {
        await foreach (var d in dataSets)
        {
            yield return Map(d);
        }
    }
}
