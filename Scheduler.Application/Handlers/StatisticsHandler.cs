// PHASE 5: Scheduler.Application/Handlers/StatisticsHandler.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Handlers;

/// <summary>Handles statistics calculation tasks.</summary>
public class StatisticsHandler : ITaskHandler
{
    private readonly IStatisticsService _statisticsService;
    private readonly ITimeSeriesRepository _timeSeriesRepository;
    private readonly ILogger<StatisticsHandler> _logger;

    /// <summary>Initializes a new instance of the <see cref="StatisticsHandler"/> class.</summary>
    public StatisticsHandler(
        IStatisticsService statisticsService,
        ITimeSeriesRepository timeSeriesRepository,
        ILogger<StatisticsHandler> logger)
    {
        _statisticsService = statisticsService;
        _timeSeriesRepository = timeSeriesRepository;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TaskExecutionResultDto> ExecuteAsync(JobContextDto context, CancellationToken cancellationToken)
    {
        var points = await _timeSeriesRepository.GetSeriesAsync(
            context.PlantId,
            context.Variables,
            context.TimeRange.StartUtc,
            context.TimeRange.EndUtc,
            cancellationToken);

        var window = context.StatisticsWindow ?? StatisticsWindow.Daily;
        var request = new StatisticsRequestDto(context.PlantId + ":" + context.Variables.Count, window)
        {
            PlantId = context.PlantId,
            Variables = new List<string>(context.Variables),
            TimeRange = context.TimeRange,
            Points = new List<TimeSeriesPointDto>(points)
        };

        var stats = await _statisticsService.CalculateAsync(request, cancellationToken);
        return new TaskExecutionResultDto(true)
        {
            Summary = $"Statistics calculation completed. Metrics={stats.Metrics.Count}."
        };
    }
}
