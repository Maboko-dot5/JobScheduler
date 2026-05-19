// PHASE E: Scheduler.Infrastructure/Configuration/SchedulerProcessingModeProvider.cs
using Microsoft.Extensions.Configuration;
using Scheduler.Application.Interfaces.Configuration;

namespace Scheduler.Infrastructure.Configuration;

/// <summary>Reads scheduler processing mode from configuration.</summary>
public class SchedulerProcessingModeProvider : ISchedulerProcessingModeProvider
{
    private readonly IConfiguration _configuration;

    /// <summary>Initializes a new instance of the <see cref="SchedulerProcessingModeProvider"/> class.</summary>
    public SchedulerProcessingModeProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public bool CompleteImmediately
    {
        get
        {
            var rawValue = _configuration["Scheduler:Processing:CompleteImmediately"];
            return bool.TryParse(rawValue, out var parsed) && parsed;
        }
    }
}
