using AsyncEnumerablePoC.Server.DataAccess;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using Microsoft.EntityFrameworkCore;

namespace AsyncEnumerablePoC.Server;

public class HistoricalDataProvider
{
    private readonly ReadDataDbContext _dbContext;

    public HistoricalDataProvider(ReadDataDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<HistoricalData> GetHistoricalData()
    {
        var histData = _dbContext.HistoricalData
            .AsNoTracking();
        return histData;
    }

    public IQueryable<HistoricalData> GetHistoricalComplexData()
    {
        var histData = _dbContext.HistoricalData
            .AsNoTracking();
        return histData;
    }

    public async IAsyncEnumerable<HistoricalData> GetHistoricalDataTransformedOnceAsyncEnumerable()
    {
        await foreach (HistoricalData data in GetHistoricalData().AsAsyncEnumerable())
        {
            yield return data with { Value =+ 1 };
        }
    }

    public async Task<IReadOnlyCollection<HistoricalData>> GetHistoricalDataTransformedOnceCollection()
    {
        var coll = (await GetHistoricalData().ToArrayAsync())
            .Select(d => d with { Value =+ 1 });
        return coll.ToArray();
    }

    public async IAsyncEnumerable<HistoricalData> GetHistoricalComplexDataTransformedOnceAsyncEnumerable()
    {
        await foreach (HistoricalData data in GetHistoricalComplexData().AsAsyncEnumerable())
        {
            yield return data with { Value =+ 1 };
        }
    }

    public async Task<IReadOnlyCollection<HistoricalData>> GetHistoricalComplexDataTransformedOnceCollection()
    {
        var coll = (await GetHistoricalComplexData().ToArrayAsync())
            .Select(d => d with { Value =+ 1 });
        return coll.ToArray();
    }

    public async IAsyncEnumerable<HistoricalData> GetHistoricalDataTransformedTwiceAsyncEnumerable()
    {
        await foreach (HistoricalData data in GetHistoricalDataTransformedOnceAsyncEnumerable())
        {
            yield return data with { Value =+ 2 };
        }
    }

    public async Task<IReadOnlyCollection<HistoricalData>> GetHistoricalDataTransformedTwiceCollection()
    {
        var coll = (await GetHistoricalDataTransformedOnceCollection())
            .Select(d => d with { Value =+ 2 });
        return coll.ToArray();
    }
}
