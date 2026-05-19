// PHASE 7: Scheduler.Infrastructure/Services/FakeAnomalyFilterService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Provides fake anomaly filtering results.</summary>
public class FakeAnomalyFilterService : IAnomalyFilterService
{
    /// <inheritdoc />
    public Task<IReadOnlyList<TimeSeriesPointDto>> FilterAsync(AnomalyFilterRequestDto request, CancellationToken cancellationToken)
    {
        if (request.Rule is null || request.Points.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<TimeSeriesPointDto>>(request.Points);
        }

        var mean = request.Points.Average(point => point.Value);
        var variance = request.Points.Average(point => Math.Pow(point.Value - mean, 2));
        var stdDev = Math.Sqrt(variance);
        var threshold = request.Rule.ZScoreThreshold * stdDev;

        var filtered = request.Points
            .Where(point => Math.Abs(point.Value - mean) <= threshold)
            .ToList();

        return Task.FromResult<IReadOnlyList<TimeSeriesPointDto>>(filtered);
    }
}
