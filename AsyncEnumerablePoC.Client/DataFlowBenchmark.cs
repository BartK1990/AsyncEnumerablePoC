using System.Net;
using AsyncEnumerablePoC.Client.DataAccess;
using AsyncEnumerablePoC.Client.DataAccess.Model;
using AsyncEnumerablePoC.Client.Receivers;
using AsyncEnumerablePoC.Server;
using AsyncEnumerablePoC.Server.DataAccess;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using BenchmarkDotNet.Attributes;
using Flurl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace AsyncEnumerablePoC.Client;

[MemoryDiagnoser]
public class DataFlowBenchmark
{
    private readonly SaveDataDbContext _saveDataDbContext;
    private readonly HistoricalDataProvider _historicalDataProvider;
    
    public HttpClient HttpClient { get; private set; }

    public DataFlowBenchmark()
    {
        HttpClient = new HttpClient();

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("connectionstrings.json", optional: false);

        IConfiguration configuration = builder.Build();

        var readDataDbContext = new ReadDataDbContext(configuration.GetConnectionString(ConnectionStrings.TheOnlyDatabase)!);
        _saveDataDbContext = new SaveDataDbContext("mongodb://localhost:27017", "AsyncEnumerablePoC_DB");
        _historicalDataProvider = new HistoricalDataProvider(readDataDbContext);
    }

    ////[Params(144, 10080, 86400, 388800)] // 1 Unit 1 day | 10 Units 1 week | 20 Units 1 month | 40 Units 3 months
    [Params(86400)]
    public int Samples;

    [GlobalSetup]
    public async Task Setup()
    {
        await CheckServerConnection();
        await FeedData(Samples);
        await ClearMongo();
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await ClearMongo();
        HttpClient.Dispose();
    }

    [Benchmark]

    public async Task<double> DbReadOnlyAsyncEnumerable()
    {
        var results = _historicalDataProvider.GetHistoricalData().AsAsyncEnumerable();
        double a = 0.0;
        await foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]

    public async Task<double> DbReadOnlyCollection()
    {
        var results = await _historicalDataProvider.GetHistoricalData().ToArrayAsync();
        double a = 0.0;
        foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]

    public async Task<double> DbReadTransformedTwiceAsyncEnumerable()
    {
        var results = _historicalDataProvider.GetHistoricalDataTransformedTwiceAsyncEnumerable();
        double a = 0.0;
        await foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]

    public async Task<double> DbReadTransformedTwiceCollection()
    {
        var results = await _historicalDataProvider.GetHistoricalDataTransformedTwiceCollection();
        double a = 0.0;
        foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }


    [Benchmark]

    public async Task<double> GetDataAsyncEnum()
    {
        var results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataAE"));

        double a = 0.0;
        await foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]

    public async Task<double> GetDataCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetDataC"));

        double a = 0.0;
        foreach (var result in results)
        {
            a = result.Value;
        }

        return a;
    }

    [Benchmark]
    public async Task GetDataTransformOnceAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed = Transform(results);

        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.InsertManyAsync(await transformed.ToArrayAsync());

        async IAsyncEnumerable<HistoricalTransformedData> Transform(IAsyncEnumerable<HistoricalData> dataSets)
        {
            await foreach (var data in dataSets)
            {
                yield return new HistoricalTransformedData(data.Timestamp, data.Value - 7);
            }
        }
    }

    [Benchmark]
    public async Task GetDataTransformOnceAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataC"));

        HistoricalTransformedData[] data = results.Select(d => new HistoricalTransformedData(d.Timestamp, d.Value - 7)).ToArray();

        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.InsertManyAsync(data);
    }

    [Benchmark]
    public async Task GetDataTransformTwiceAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = Transform1(results, -7);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = Transform2(transformed1, 3);

        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.InsertManyAsync(await transformed2.ToArrayAsync());

        async IAsyncEnumerable<HistoricalTransformedData> Transform1(IAsyncEnumerable<HistoricalData> dataSets, int val)
        {
            await foreach (var data in dataSets)
            {
                yield return new HistoricalTransformedData(data.Timestamp, data.Value + val);
            }
        }

        async IAsyncEnumerable<HistoricalTransformedData> Transform2(IAsyncEnumerable<HistoricalTransformedData> dataSets, int val)
        {
            await foreach (var data in dataSets)
            {
                yield return data with { Value = data.Value + val };
            }
        }
    }

    [Benchmark]
    public async Task GetDataTransformTwiceAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataC"));

        HistoricalTransformedData[] data1 = results.Select(d => new HistoricalTransformedData(d.Timestamp, d.Value - 7)).ToArray();

        HistoricalTransformedData[] data2 = data1.Select(d => d with { Value = d.Value + 3 }).ToArray();

        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.InsertManyAsync(data2);
    }

    [Benchmark]
    public async Task GetDataTransformThreeAndSaveAsyncEnum()
    {
        IAsyncEnumerable<HistoricalData> results = AsyncEnumerableReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataAE"));

        IAsyncEnumerable<HistoricalTransformedData> transformed1 = Transform1(results, -7);
        IAsyncEnumerable<HistoricalTransformedData> transformed2 = Transform2(transformed1, 3);
        IAsyncEnumerable<HistoricalTransformedData> transformed3 = Transform2(transformed2, 4);

        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.InsertManyAsync(await transformed3.ToArrayAsync());

        async IAsyncEnumerable<HistoricalTransformedData> Transform1(IAsyncEnumerable<HistoricalData> dataSets, int val)
        {
            await foreach (var data in dataSets)
            {
                yield return new HistoricalTransformedData(data.Timestamp, data.Value + val);
            }
        }

        async IAsyncEnumerable<HistoricalTransformedData> Transform2(IAsyncEnumerable<HistoricalTransformedData> dataSets, int val)
        {
            await foreach (var data in dataSets)
            {
                yield return data with { Value = data.Value + val };
            }
        }
    }

    [Benchmark]
    public async Task GetDataTransformThreeAndSaveCollection()
    {
        var results = await CollectionReceiver.RequestData<HistoricalData>(
            HttpClient,
            Url.Combine("HistoricalData", "GetTransformedOnceDataC"));

        HistoricalTransformedData[] data1 = results.Select(d => new HistoricalTransformedData(d.Timestamp, d.Value - 7)).ToArray();
        HistoricalTransformedData[] data2 = data1.Select(d => d with { Value = d.Value + 3 }).ToArray();
        HistoricalTransformedData[] data3 = data2.Select(d => d with { Value = d.Value + 3 }).ToArray();

        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.InsertManyAsync(data3);
    }

    private static async Task CheckServerConnection()
    {
        using HttpClient httpClient = new();

        bool success;
        int i = 0;
        do
        {
            Console.WriteLine($"Attempt to connect ... {++i}");

            using HttpResponseMessage response = await httpClient.GetAsync(
                Url.Combine(IpAddress.Localhost, "HistoricalData", "Connect")).ConfigureAwait(false);

            success = response.StatusCode == HttpStatusCode.OK;

            await Task.Delay(2000);
        } while (!success);

        Console.WriteLine("Connected!");
    }

    private static async Task FeedData(int samples)
    {
        using HttpClient httpClient = new();

        using HttpResponseMessage response = await httpClient.PutAsync(
            Url.Combine(IpAddress.Localhost, "HistoricalData", "InsertData", samples.ToString()), null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode(); // It will throw if not success
    }

    private async Task ClearMongo()
    {
        await _saveDataDbContext.DbSet.HistoricalTransformedDataSets.DeleteManyAsync(
            Builders<HistoricalTransformedData>.Filter.Empty); // Delete all

        await _saveDataDbContext.DbSet.HistoricalTransformedComplexDataSets.DeleteManyAsync(
            Builders<HistoricalTransformedComplexData>.Filter.Empty); // Delete all
    }
}
