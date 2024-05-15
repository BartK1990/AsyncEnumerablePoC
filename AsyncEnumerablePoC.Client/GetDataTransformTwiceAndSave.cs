using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;
using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client;

public class GetDataTransformTwiceAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetDataTransformTwiceAndSave")]
    public async Task GetDataAndSaveBatchesCollection()
    {
        int count = Samples / BatchSize;
        for (int i = 0; i < count; i++)
        {
            IReadOnlyCollection<HistoricalData> results = await CollectionReceiver.PostData<HistoricalData>(
                HttpClient,
                Url.Combine("HistoricalData", "GetDataC-Batch"),
                new GetDataBatchRequest { BatchCount = i, BatchSize = BatchSize });

            var mapped = results.Select(Map);
            var transformed1 = mapped.Select(d => Transform(d, 123.3));
            var transformed2 = transformed1.Select(d => Transform(d, -321.1));

            await MongoDataSet.InsertManyAsync(transformed2);
        }
    }

    [Benchmark, BenchmarkCategory("GetDataTransformTwiceAndSave")]
    public async Task GetDataAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.PostData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"),
            new GetDataBatchRequest { BatchCount = 0, BatchSize = BatchSize });

        var mapped = results.Select(Map);
        var transformed1 = mapped.Select(d => Transform(d, 123.3));
        var transformed2 = transformed1.Select(d => Transform(d, -321.1));

        await BatchInsert(transformed2, MongoDataSet);
    }
}
