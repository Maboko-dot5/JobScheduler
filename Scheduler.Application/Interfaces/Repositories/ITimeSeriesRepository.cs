// PHASE 2: Scheduler.Application/Interfaces/Repositories/ITimeSeriesRepository.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Interfaces.Repositories;

/// <summary>Defines data access operations for time series data.</summary>
public interface ITimeSeriesRepository
{
    /// <summary>Gets time series data for a plant and variables within a time range.</summary>
    Task<IReadOnlyList<TimeSeriesPointDto>> GetSeriesAsync(
        string plantId,
        IReadOnlyList<string> variables,
        DateTimeOffset startUtc,
        DateTimeOffset endUtc,
        CancellationToken cancellationToken);
}
