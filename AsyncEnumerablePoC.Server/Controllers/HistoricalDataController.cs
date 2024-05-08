using Microsoft.AspNetCore.Mvc;

namespace AsyncEnumerablePoC.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class HistoricalDataController : ControllerBase
{

    private readonly ILogger<HistoricalDataController> _logger;

    public HistoricalDataController(ILogger<HistoricalDataController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "InsertData")]
    public ActionResult Get()
    {
        return Ok();
    }
}