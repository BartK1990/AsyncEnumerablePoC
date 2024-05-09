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
        return _historicalDataProvider.GetHistoricalData().AsAsyncEnumerable();
    }

    [HttpGet("GetDataC")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(IReadOnlyCollection<HistoricalData>), StatusCodes.Status200OK)]
    public ActionResult GetHistoricalDataCollection()
    {
        return Ok(_historicalDataProvider.GetHistoricalData().ToArray());
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