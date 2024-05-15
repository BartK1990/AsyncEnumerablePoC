using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;

public class Get1DataAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory(nameof(Get1DataAndSave))]
    public async Task GetBatchesCollection()
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

    [Benchmark, BenchmarkCategory(nameof(Get1DataAndSave))]
    public async Task GetAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.PostData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"),
            new GetDataBatchRequest { BatchCount = 0, BatchSize = BatchSize });

        await BatchInsert(results.Select(Map), MongoDataSet);
    }
}
