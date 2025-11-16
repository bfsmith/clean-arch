using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CleanArch.Logging;

namespace CleanArch.API.Controllers;

[Route("[controller]")]
public class EchoController : Controller
{
    private readonly ILogger<EchoController> _logger;

    public EchoController(ILogger<EchoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public ActionResult<string> Echo([FromQuery] string message)
    {
        _logger.Info("Echo endpoint called", new { text = message });
        return Ok(message);
    }
}
