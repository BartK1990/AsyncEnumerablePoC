using AsyncEnumerablePoC.Server.DataAccess;
using AsyncEnumerablePoC.Server.DataAccess.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AsyncEnumerablePoC.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class HistoricalDataController : ControllerBase
{
    private readonly ILogger<HistoricalDataController> _logger;
    private readonly ReadDataDbContext _dataDbContext;
    private readonly HistoricalDataProvider _historicalDataProvider;

    public HistoricalDataController(ILogger<HistoricalDataController> logger, ReadDataDbContext dataDbContext, HistoricalDataProvider historicalDataProvider)
    {
        _logger = logger;
        _dataDbContext = dataDbContext;
        _historicalDataProvider = historicalDataProvider;
    }

    [HttpGet("Connect")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult Connect()
    {
        return Ok();
    }

    [HttpGet("GetDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GetHistoricalDataAsyncEnumerable()
    {
        return _historicalDataProvider.GetHistoricalData()
            .OrderBy(h => h.Timestamp)
            .AsAsyncEnumerable();
    }

    [HttpPost("GetDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GetPostHistoricalDataAsyncEnumerable([FromBody] GetDataBatchRequest request)
    {
        return _historicalDataProvider.GetHistoricalData()
            .OrderBy(h => h.Timestamp)
            .AsAsyncEnumerable();
    }

    [HttpGet("GetDataC")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetHistoricalDataCollection()
    {
        return Ok(await _historicalDataProvider.GetHistoricalData()
            .OrderBy(h => h.Timestamp)
            .ToArrayAsync());
    }

    [HttpPost("GetDataC-Batch")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetHistoricalDataCollectionBatch([FromBody] GetDataBatchRequest request)
    {
        return Ok(await _historicalDataProvider.GetHistoricalData()
            .OrderBy(h => h.Timestamp)
            .Skip(request.BatchCount * request.BatchSize)
            .Take(request.BatchSize)
            .ToArrayAsync());
    }

    [HttpGet("GetComplexDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GetHistoricalComplexDataAsyncEnumerable()
    {
        return _historicalDataProvider.GetHistoricalComplexData()
            .OrderBy(h => h.Timestamp)
            .AsAsyncEnumerable();
    }

    [HttpPost("GetComplexDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GetPostHistoricalComplexDataAsyncEnumerable([FromBody] GetDataBatchRequest request)
    {
        return _historicalDataProvider.GetHistoricalComplexData()
            .OrderBy(h => h.Timestamp)
            .AsAsyncEnumerable();
    }

    [HttpGet("GetComplexDataC")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetHistoricalComplexDataCollection()
    {
        return Ok(await _historicalDataProvider.GetHistoricalComplexData()
            .OrderBy(h => h.Timestamp)
            .ToArrayAsync());
    }

    [HttpPost("GetComplexDataC-Batch")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetHistoricalComplexDataCollectionBatch([FromBody] GetDataBatchRequest request)
    {
        return Ok(await _historicalDataProvider.GetHistoricalComplexData()
            .OrderBy(h => h.Timestamp)
            .Skip(request.BatchCount * request.BatchSize)
            .Take(request.BatchSize)
            .ToArrayAsync());
    }

    [HttpGet("GetTransformedOnceDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GetHistoricalTransformedOnceDataDataAsyncEnumerable()
    {
        return _historicalDataProvider.GetHistoricalDataTransformedOnceAsyncEnumerable();
    }

    [HttpGet("GetTransformedOnceDataC")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetHistoricalTransformedOnceDataDataCollection()
    {
        return Ok(await _historicalDataProvider.GetHistoricalDataTransformedOnceCollection());
    }

    [HttpGet("GetComplexTransformedOnceDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GetHistoricalComplexTransformedOnceDataDataAsyncEnumerable()
    {
        _logger.LogWarning("AsyncE");
        var result = _historicalDataProvider.GetHistoricalComplexDataTransformedOnceAsyncEnumerable();
        return result;
    }

    [HttpGet("GetComplexTransformedOnceDataC")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetHistoricalComplexTransformedOnceDataDataCollection()
    {
        return Ok(await _historicalDataProvider.GetHistoricalComplexDataCollection());
    }

    [HttpGet("GC-GetDataAE")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public IAsyncEnumerable<object> GcGetDataAsyncEnumerable()
    {
        _logger.LogWarning("AsyncE");
        var result = _historicalDataProvider.GetHistoricalComplexDataTransformedOnceAsyncEnumerable();
        return result;
    }

    [HttpGet("GC-GetDataC")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalComplexData>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GcGetDataCollection()
    {
        _logger.LogWarning("Collection");
        var memory = GC.GetTotalMemory(true);
        _logger.LogWarning("Memory C before: {Memory}", memory);
        var result = await _historicalDataProvider.GetHistoricalComplexDataCollection();
        _logger.LogError("Memory C offset: {Memory}", _historicalDataProvider.Memory - memory);
        return Ok(result);
    }

    [HttpPut("InsertData/{dataCount:int}")]
    public ActionResult InsertHistoricalData(int dataCount)
    {
        _dataDbContext.Database.ExecuteSqlRaw($"TRUNCATE TABLE [{nameof(HistoricalData)}]");
        _dataDbContext.Database.ExecuteSqlRaw($"TRUNCATE TABLE [{nameof(HistoricalComplexData)}]");
        Random rnd = new Random();
        for (int i = 0; i < dataCount; i++)
        {
            _dataDbContext.HistoricalData.Add(new HistoricalData
            {
                Timestamp = DateTime.Now,
                Value = rnd.NextDouble(),
            });

            _dataDbContext.HistoricalComplexData.Add(new HistoricalComplexData
            {
                Timestamp = DateTime.Now,
                Value1 = rnd.NextDouble(),
                Value2 = rnd.NextDouble(),
                Value3 = rnd.NextDouble(),
                Value4 = rnd.NextDouble(),
                Value5 = rnd.NextDouble(),
            });
        }

        _dataDbContext.SaveChanges();
        _dataDbContext.ChangeTracker.Clear(); 
        
        return Ok();
    }
}