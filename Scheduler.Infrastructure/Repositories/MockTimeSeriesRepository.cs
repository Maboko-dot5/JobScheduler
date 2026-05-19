// PHASE 7: Scheduler.Infrastructure/Repositories/MockTimeSeriesRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Repositories;

namespace Scheduler.Infrastructure.Repositories;

/// <summary>Provides mock time series data.</summary>
public class MockTimeSeriesRepository : ITimeSeriesRepository
{
    /// <inheritdoc />
    public Task<IReadOnlyList<TimeSeriesPointDto>> GetSeriesAsync(
        string plantId,
        IReadOnlyList<string> variables,
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        CancellationToken cancellationToken)
    {
        var result = new List<TimeSeriesPointDto>();

        if (variables.Count == 0 || endUtc <= startUtc)
        {
            return Task.FromResult<IReadOnlyList<TimeSeriesPointDto>>(result);
        }

        var current = startUtc;
        var index = 0;
        while (current <= endUtc)
        {
            foreach (var variable in variables)
            {
                var baseValue = GetBaseValue(plantId, variable);
                var value = baseValue + Math.Sin(index / 10.0) * 5.0;
                result.Add(new TimeSeriesPointDto(current, variable, value));
            }

            current = current.AddMinutes(1);
            index++;
        }

        return Task.FromResult<IReadOnlyList<TimeSeriesPointDto>>(result);
    }

    private static double GetBaseValue(string plantId, string variable)
    {
        var hash = (plantId + ":" + variable).GetHashCode();
        var normalized = Math.Abs(hash % 50);

        return variable.ToLowerInvariant() switch
        {
            "temperature" => 50 + normalized,
            "pressure" => 150 + normalized,
            "vibration" => 10 + normalized / 2.0,
            _ => normalized
        };
    }
}
