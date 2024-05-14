using AsyncEnumerablePoC.Server.DataAccess;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace AsyncEnumerablePoC.Server;

public class HistoricalDataProvider
{
    private readonly ReadDataDbContext _dbContext;
    private readonly ILogger<ReadDataDbContext> _logger;

    public long Memory { get; set; }

    public HistoricalDataProvider(ReadDataDbContext dbContext, ILogger<ReadDataDbContext> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public IQueryable<HistoricalData> GetHistoricalData()
    {
        var histData = _dbContext.HistoricalData
            .AsNoTracking();
        return histData;
    }

    public IQueryable<HistoricalComplexData> GetHistoricalComplexData()
    {
        var histData = _dbContext.HistoricalComplexData
            .AsNoTracking();
        return histData;
    }

    public async Task<IReadOnlyCollection<HistoricalData>> GetHistoricalDataTransformedOnceCollection()
    {
        var coll = (await GetHistoricalData().ToArrayAsync())
            .Select(d => d with { Value =+ 1 });
        return coll.ToArray();
    }

    public async IAsyncEnumerable<HistoricalData> GetHistoricalDataTransformedOnceAsyncEnumerable()
    {
        await foreach (HistoricalData data in GetHistoricalData().AsAsyncEnumerable())
        {
            yield return data with { Value =+ 1 };
        }
    }

    public async Task<IReadOnlyCollection<HistoricalComplexData>> GetHistoricalComplexDataCollection()
    {
        return await GetHistoricalComplexData().ToArrayAsync();
    }

    public IAsyncEnumerable<HistoricalComplexData> GetHistoricalDataTransformedTwiceAsyncEnumerable()
    {
        return GetHistoricalComplexData().AsAsyncEnumerable();
    }

    public async IAsyncEnumerable<HistoricalComplexData> GetHistoricalComplexDataTransformedOnceAsyncEnumerable()
    {
        await foreach (HistoricalComplexData data in GetHistoricalDataTransformedTwiceAsyncEnumerable())
        {
            yield return data with 
            { 
                Value1 = +1 ,
                Value2 = +1 ,
                Value3 = +1 ,
                Value4 = +1 ,
                Value5 = +1 , 
            };
        }
    }

    public async IAsyncEnumerable<HistoricalData> GcGetDataAsyncEnumerable()
    {
        Memory = 0;
        var memory = GC.GetTotalMemory(true);
        _logger.LogWarning("Memory before: {Memory}", memory);

        IAsyncEnumerator<HistoricalData>? enumerator = GetHistoricalData().AsAsyncEnumerable().GetAsyncEnumerator();
        try
        {
            while (await enumerator.MoveNextAsync())
            {
                var currentMemory = GC.GetTotalMemory(true);
                if (Memory < currentMemory)
                {
                    Memory = currentMemory;
                    _logger.LogWarning("Memory AE offset: {Memory}", Memory - memory);
                }
                yield return enumerator.Current with { Value =+ 1 };
            }
        }
        finally { 
            if (enumerator is not null)
            {
                await enumerator.DisposeAsync();
            }

            _logger.LogError("Memory AE offset: {Memory}", Memory - memory);
        }
    }
}
