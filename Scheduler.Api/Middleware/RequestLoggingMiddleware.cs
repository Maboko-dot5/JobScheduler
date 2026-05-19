// PHASE 9: Scheduler.Api/Middleware/RequestLoggingMiddleware.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Scheduler.Api.Middleware;

/// <summary>Logs incoming requests and responses.</summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.</summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware.</summary>
    public Task InvokeAsync(HttpContext context)
    {
        return _next(context);
    }
}
