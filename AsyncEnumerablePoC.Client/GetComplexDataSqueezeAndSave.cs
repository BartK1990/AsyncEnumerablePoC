using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;

namespace AsyncEnumerablePoC.Client;

public class GetComplexDataSqueezeAndSave : HistoricalDataBenchmark
{
    [Benchmark(Baseline = true), BenchmarkCategory("GetComplexDataSqueezeAndSave")]
    public async Task GetDataTransformThreeAndSaveBatchesCollection()
    {
        int count = Samples / BatchSize;
        for (int i = 0; i < count; i++)
        {
            var results = await CollectionReceiver.PostData<HistoricalComplexData>(
                HttpClient,
                Url.Combine("HistoricalData", "GetComplexDataC-Batch"),
                new GetDataBatchRequest { BatchCount = i, BatchSize = BatchSize });

            IEnumerable<HistoricalTransformedData> squeezed = results.Select(d => new HistoricalTransformedData(
                d.Timestamp, 
                d.Value1 + d.Value2 - d.Value3 * d.Value4 + d.Value5));

            await MongoDataSet.InsertManyAsync(squeezed);
        }
    }

    [Benchmark, BenchmarkCategory("GetComplexDataSqueezeAndSave")]
    public async Task GetDataTransformThreeAndSaveBatchesAsyncEnum()
    {
        IAsyncEnumerable<HistoricalComplexData> results = AsyncEnumerableReceiver.RequestData<HistoricalComplexData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetComplexDataAE"));

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
