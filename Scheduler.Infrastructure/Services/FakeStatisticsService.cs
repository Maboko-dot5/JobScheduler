// PHASE 7: Scheduler.Infrastructure/Services/FakeStatisticsService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Provides fake statistics results.</summary>
public class FakeStatisticsService : IStatisticsService
{
    /// <inheritdoc />
    public Task<StatisticsResultDto> CalculateAsync(StatisticsRequestDto request, CancellationToken cancellationToken)
    {
        var result = new StatisticsResultDto(request.SeriesId)
        {
            SeriesId = request.SeriesId
        };

        result.Metrics["count"] = request.Points.Count;
        if (request.Points.Count == 0)
        {
            return Task.FromResult(result);
        }

        result.Metrics["min"] = request.Points.Min(point => point.Value);
        result.Metrics["max"] = request.Points.Max(point => point.Value);
        result.Metrics["avg"] = request.Points.Average(point => point.Value);

        foreach (var group in request.Points.GroupBy(point => point.Variable ?? string.Empty))
        {
            var key = string.IsNullOrWhiteSpace(group.Key) ? "unknown" : group.Key.ToLowerInvariant();
            result.Metrics[$"{key}.count"] = group.Count();
            result.Metrics[$"{key}.min"] = group.Min(point => point.Value);
            result.Metrics[$"{key}.max"] = group.Max(point => point.Value);
            result.Metrics[$"{key}.avg"] = group.Average(point => point.Value);
        }

        var inTargetCount = request.Points.Count(point => IsInTarget(point));
        var inTargetPct = (double)inTargetCount / request.Points.Count * 100.0;
        result.Metrics["inTargetPct"] = inTargetPct;
        result.Metrics["uptimePct"] = inTargetPct;

        if (request.TimeRange is not null)
        {
            var windowHours = GetWindowHours(request.Window);
            var durationHours = (request.TimeRange.EndUtc - request.TimeRange.StartUtc).TotalHours;
            if (durationHours > 0 && windowHours > 0)
            {
                var windowCount = Math.Ceiling(durationHours / windowHours);
                result.Metrics["windowHours"] = windowHours;
                result.Metrics["windowCount"] = windowCount;
            }
        }

        return Task.FromResult(result);
    }

    private static bool IsInTarget(TimeSeriesPointDto point)
    {
        var (min, max) = GetTargetRange(point.Variable);
        return point.Value >= min && point.Value <= max;
    }

    private static (double min, double max) GetTargetRange(string? variable)
    {
        var key = variable?.ToLowerInvariant() ?? string.Empty;
        return key switch
        {
            "temperature" => (10, 90),
            "pressure" => (90, 250),
            "vibration" => (0, 25),
            _ => (-1_000_000, 1_000_000)
        };
    }

    private static double GetWindowHours(Scheduler.Domain.Enums.StatisticsWindow window)
    {
        return window switch
        {
            Scheduler.Domain.Enums.StatisticsWindow.Shift => 8,
            Scheduler.Domain.Enums.StatisticsWindow.Daily => 24,
            Scheduler.Domain.Enums.StatisticsWindow.Weekly => 168,
            _ => 24
        };
    }
}
