// PHASE 5: Scheduler.Application/Handlers/AnomalyFilterHandler.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Application.Handlers;

/// <summary>Handles anomaly filtering tasks.</summary>
public class AnomalyFilterHandler : ITaskHandler
{
    private readonly IAnomalyFilterService _anomalyFilterService;
    private readonly IFilteringRuleProvider _filteringRuleProvider;
    private readonly ITimeSeriesRepository _timeSeriesRepository;
    private readonly ILogger<AnomalyFilterHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="AnomalyFilterHandler"/> class.</summary>
    public AnomalyFilterHandler(
        IAnomalyFilterService anomalyFilterService,
        IFilteringRuleProvider filteringRuleProvider,
        ITimeSeriesRepository timeSeriesRepository,
        ILogger<AnomalyFilterHandler> logger)
    {
        _anomalyFilterService = anomalyFilterService;
        _filteringRuleProvider = filteringRuleProvider;
        _timeSeriesRepository = timeSeriesRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TaskExecutionResultDto> ExecuteAsync(JobContextDto context, CancellationToken cancellationToken)
    {
        var variable = context.Variables.Count > 0 ? context.Variables[0] : string.Empty;
        var rule = await _filteringRuleProvider.GetAnomalyRuleAsync(context.PlantId, variable, cancellationToken);
        var points = await _timeSeriesRepository.GetSeriesAsync(
            context.PlantId,
            context.Variables,
            context.TimeRange.StartUtc,
            context.TimeRange.EndUtc,
            cancellationToken);

        var request = new AnomalyFilterRequestDto(context.PlantId + ":" + variable)
        {
            PlantId = context.PlantId,
            Variable = variable,
            Rule = rule,
            Points = new List<TimeSeriesPointDto>(points)
        };

        var filtered = await _anomalyFilterService.FilterAsync(request, cancellationToken);
        var removed = request.Points.Count - filtered.Count;
        return new TaskExecutionResultDto(true)
        {
            Summary = $"Removed {removed} anomalous readings."
        };
    }
}
