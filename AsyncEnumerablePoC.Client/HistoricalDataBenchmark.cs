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
    public const int BatchSize = 10_000;

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
        HistoricalDataProvider = new HistoricalDataProvider(readDataDbContext);
    }

    //[Params(144, 10080, 86400, 388800)] // 1 Unit 1 day | 10 Units 1 week | 20 Units 1 month | 40 Units 3 months
    [Params(10080)]
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
        return data with
        {
            Value =+ val,
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
}
