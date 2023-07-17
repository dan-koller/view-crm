using Microsoft.AspNetCore.Mvc; // [Route], [ApiController], ControllerBase

namespace ViewBoard.WebApi.Controllers;

// base address: api/health
[Route(Constants.ApiRoute)]
[ApiController]
public class HealthController : ControllerBase
{
    // GET: api/health
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
}

