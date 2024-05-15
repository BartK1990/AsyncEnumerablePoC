using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;

public class Get3ComplexDataSqueezeAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory(nameof(Get3ComplexDataSqueezeAndSave))]
    public async Task GetBatchesCollection()
    {
        int count = Samples / BatchSize;
        for (int i = 0; i < count; i++)
        {
            var results = await CollectionReceiver.PostData<HistoricalComplexData>(
                HttpClient,
                Url.Combine("HistoricalData", "GetComplexDataC-Batch"),
                new GetDataBatchRequest { BatchCount = i, BatchSize = BatchSize });

            var mapped = results.Select(Map);
            var squeezed = mapped.Select(d => new HistoricalTransformedData(
                d.Timestamp, 
                d.Value1 + d.Value2 - d.Value3 * d.Value4 + d.Value5));

            await MongoDataSet.InsertManyAsync(squeezed);
        }
    }

    [Benchmark, BenchmarkCategory(nameof(Get3ComplexDataSqueezeAndSave))]
    public async Task GetAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.PostData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexDataAE"),
            new GetDataBatchRequest { BatchCount = 0, BatchSize = BatchSize });

        var mapped = results.Select(Map);
        var squeezed = mapped.Select(d => new HistoricalTransformedData(
            d.Timestamp, 
            d.Value1 + d.Value2 - d.Value3 * d.Value4 + d.Value5));

        await BatchInsert(squeezed, MongoDataSet);
    }
}
