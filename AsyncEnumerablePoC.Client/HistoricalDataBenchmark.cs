using AsyncEnumerablePoC.Client.DataAccess;
using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client;

[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
[MemoryDiagnoser]
public abstract class HistoricalDataBenchmark
{
    public const int BatchSize = 1000;

    protected readonly SaveDataDbContext SaveDataDbContext;
    protected readonly HistoricalDataProvider HistoricalDataProvider;
    
    public HttpClient HttpClient { get; }

    public virtual IMongoCollection<HistoricalTransformedData> MongoDataSet =>
        SaveDataDbContext.DbSet.HistoricalTransformedDataSets;

    public HistoricalDataBenchmark()
    {
        HttpClient = new HttpClient();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("connectionstrings.json", optional: false);

        IConfiguration configuration = builder.Build();

        var readDataDbContext = new ReadDataDbContext(configuration.GetConnectionString(ConnectionStrings.TheOnlyDatabase)!);
        SaveDataDbContext = new SaveDataDbContext(ConnectionHelper.MongoDbLocalhost, ConnectionHelper.MongoDbName);
        HistoricalDataProvider = new HistoricalDataProvider(readDataDbContext, null);
    }

    //[Params(144, 10080, 86400, 388800)] // 1 Unit 1 day | 10 Units 1 week | 20 Units 1 month | 40 Units 3 months
    //[Params(1000, 10000, 86000, 388000)] // Check the limit of Large object Heap - 85 000 bytes
    [Params(1000)]
    public int Samples;

    [GlobalSetup]
    public async Task Setup()
    {
        await ConnectionHelper.CheckServerConnection(HttpClient);
        await ConnectionHelper.FeedData(HttpClient, Samples);
        await ClearMongo();
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await ClearMongo();
        HttpClient.Dispose();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        ClearMongo().Wait();
    }

    protected static async IAsyncEnumerable<HistoricalTransformedData> MapTransformAsyncEnum(IAsyncEnumerable<HistoricalData> dataSets, double val)
    {
        await foreach (var data in dataSets)
        {
            yield return Transform(Map(data), val);
        }
    }

    protected static async IAsyncEnumerable<HistoricalTransformedData> TransformAsyncEnum(IAsyncEnumerable<HistoricalTransformedData> dataSets, double val)
    {
        await foreach (var data in dataSets)
        {
            yield return Transform(data, val);
        }
    }

    protected static HistoricalTransformedData Map(HistoricalData data)
    {
        return new HistoricalTransformedData(
            data.Timestamp,
            data.Value);
    }

    protected static HistoricalTransformedData Transform(HistoricalTransformedData data, double val)
    {
        AdditionalEffort();
        return data with
        {
            Value =+ val,
        };
    }

    protected static IAsyncEnumerable<HistoricalTransformedComplexData> MapTransform(IAsyncEnumerable<HistoricalComplexData> dataSets, double val)
    {
        return dataSets.Select(data => Transform(Map(data), val));
    }

    protected static async IAsyncEnumerable<HistoricalTransformedComplexData> Transform(IAsyncEnumerable<HistoricalTransformedComplexData> dataSets, double val)
    {
        AdditionalEffort();
        await foreach (var data in dataSets)
        {
            yield return Transform(data, val);
        }
    }

    protected static HistoricalTransformedComplexData Map(HistoricalComplexData data)
    {
        return new HistoricalTransformedComplexData(
            data.Timestamp,
            data.Value1,
            data.Value2,
            data.Value3,
            data.Value4,
            data.Value5);
    }

    protected static HistoricalTransformedComplexData Transform(HistoricalTransformedComplexData data, double val)
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

    protected static async Task BatchInsert<T>(IAsyncEnumerable<T> transformed,
        IMongoCollection<T> mongoDataSet)
    {
        var batch = new List<T>();
        await foreach (T data in transformed)
        {
            batch.Add(data);
            if (batch.Count < BatchSize) continue;
            await mongoDataSet.InsertManyAsync(batch);
            batch.Clear();
        }

        if (batch.Any())
        {
            await mongoDataSet.InsertManyAsync(batch);
        }
    }

    protected virtual async Task ClearMongo()
    {
        await MongoDataSet.DeleteManyAsync(
            Builders<HistoricalTransformedData>.Filter.Empty); // Delete all
    }

    private static void AdditionalEffort()
    {
    }
}
