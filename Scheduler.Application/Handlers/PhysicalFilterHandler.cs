// PHASE 5: Scheduler.Application/Handlers/PhysicalFilterHandler.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Application.Handlers;

/// <summary>Handles physical filtering tasks.</summary>
public class PhysicalFilterHandler : ITaskHandler
{
    private readonly IPhysicalFilterService _physicalFilterService;
    private readonly IFilteringRuleProvider _filteringRuleProvider;
    private readonly ITimeSeriesRepository _timeSeriesRepository;
    private readonly ILogger<PhysicalFilterHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="PhysicalFilterHandler"/> class.</summary>
    public PhysicalFilterHandler(
        IPhysicalFilterService physicalFilterService,
        IFilteringRuleProvider filteringRuleProvider,
        ITimeSeriesRepository timeSeriesRepository,
        ILogger<PhysicalFilterHandler> logger)
    {
        _physicalFilterService = physicalFilterService;
        _filteringRuleProvider = filteringRuleProvider;
        _timeSeriesRepository = timeSeriesRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TaskExecutionResultDto> ExecuteAsync(JobContextDto context, CancellationToken cancellationToken)
    {
        var variable = context.Variables.Count > 0 ? context.Variables[0] : string.Empty;
        var rule = await _filteringRuleProvider.GetPhysicalRuleAsync(context.PlantId, variable, cancellationToken);
        var points = await _timeSeriesRepository.GetSeriesAsync(
            context.PlantId,
            context.Variables,
            context.TimeRange.StartUtc,
            context.TimeRange.EndUtc,
            cancellationToken);

        var request = new PhysicalFilterRequestDto(context.PlantId + ":" + variable)
        {
            PlantId = context.PlantId,
            Variable = variable,
            Rule = rule,
            Points = new List<TimeSeriesPointDto>(points)
        };

        var filtered = await _physicalFilterService.FilterAsync(request, cancellationToken);
        var removed = request.Points.Count - filtered.Count;
        var displayVariable = string.IsNullOrWhiteSpace(variable) ? "variable" : variable;
        return new TaskExecutionResultDto(true)
        {
            Summary = $"Filtered {removed} outliers from {displayVariable}."
        };
    }
}
