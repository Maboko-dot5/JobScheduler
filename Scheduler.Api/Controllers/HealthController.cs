// PHASE B: Scheduler.Api/Controllers/HealthController.cs
using Microsoft.AspNetCore.Mvc;

namespace Scheduler.Api.Controllers;

/// <summary>Provides service health information.</summary>
[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    /// <summary>Gets service health and version details.</summary>
    [HttpGet]
    public ActionResult GetAsync()
    {
        var payload = new
        {
            status = "Healthy",
            version = typeof(HealthController).Assembly.GetName().Version?.ToString() ?? "unknown"
        };

        return Ok(payload);
    }
}
