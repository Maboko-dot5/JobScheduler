// PHASE 5: Scheduler.Application/Handlers/StatisticsHandler.cs
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
            Summary = BuildSummary(window, stats, context.Variables)
        };
    }

    private static string BuildSummary(
        StatisticsWindow window,
        StatisticsResultDto statistics,
        IReadOnlyList<string> variables)
    {
        var builder = new StringBuilder();
        builder.Append(window);
        builder.Append(" statistics: count=");
        builder.Append(FormatMetric(statistics.Metrics, "count"));
        builder.Append(", min=");
        builder.Append(FormatMetric(statistics.Metrics, "min"));
        builder.Append(", max=");
        builder.Append(FormatMetric(statistics.Metrics, "max"));
        builder.Append(", avg=");
        builder.Append(FormatMetric(statistics.Metrics, "avg"));
        builder.Append(", uptime=");
        builder.Append(FormatMetric(statistics.Metrics, "uptimePct"));
        builder.Append("%, in target=");
        builder.Append(FormatMetric(statistics.Metrics, "inTargetPct"));
        builder.Append('%');

        if (statistics.Metrics.ContainsKey("windowCount") || statistics.Metrics.ContainsKey("windowHours"))
        {
            builder.Append(", windows=");
            builder.Append(FormatMetric(statistics.Metrics, "windowCount"));
            builder.Append(" x ");
            builder.Append(FormatMetric(statistics.Metrics, "windowHours"));
            builder.Append('h');
        }

        foreach (var variable in variables)
        {
            var key = variable.ToLowerInvariant();
            if (!statistics.Metrics.ContainsKey(key + ".count"))
            {
                continue;
            }

            builder.Append(". ");
            builder.Append(variable);
            builder.Append(": count=");
            builder.Append(FormatMetric(statistics.Metrics, key + ".count"));
            builder.Append(", min=");
            builder.Append(FormatMetric(statistics.Metrics, key + ".min"));
            builder.Append(", max=");
            builder.Append(FormatMetric(statistics.Metrics, key + ".max"));
            builder.Append(", avg=");
            builder.Append(FormatMetric(statistics.Metrics, key + ".avg"));
        }

        return builder.ToString();
    }

    private static string FormatMetric(IReadOnlyDictionary<string, double> metrics, string key)
    {
        return metrics.TryGetValue(key, out var value)
            ? value.ToString("0.###", CultureInfo.InvariantCulture)
            : "n/a";
    }
}
