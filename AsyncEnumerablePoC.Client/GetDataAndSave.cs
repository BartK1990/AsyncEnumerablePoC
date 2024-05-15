using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;

public class GetDataAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetDataMapAndSave")]
    public async Task GetDataAndSaveBatchesCollection()
    {
        int count = Samples / BatchSize;
        for (int i = 0; i < count; i++)
        {
            IReadOnlyCollection<HistoricalData> results = await CollectionReceiver.PostData<HistoricalData>(
                HttpClient,
                Url.Combine("HistoricalData", "GetDataC-Batch"),
                new GetDataBatchRequest { BatchCount = i, BatchSize = BatchSize });

            await MongoDataSet.InsertManyAsync(results.Select(Map));
        }
    }

    [Benchmark, BenchmarkCategory("GetDataMapAndSave")]
    public async Task GetDataAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.PostData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"),
            new GetDataBatchRequest { BatchCount = 0, BatchSize = BatchSize });

        await BatchInsert(results.Select(Map), MongoDataSet);
    }
}
