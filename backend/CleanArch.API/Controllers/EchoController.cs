using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

[Route("[controller]")]
public class EchoController : Controller
{
    [HttpGet]
    public ActionResult<string> Echo([FromQuery] string message)
    {
        return Ok(message);
    }
}
