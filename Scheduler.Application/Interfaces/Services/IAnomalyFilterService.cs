// PHASE 2: Scheduler.Application/Interfaces/Services/IAnomalyFilterService.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines anomaly filtering operations.</summary>
public interface IAnomalyFilterService
{
    /// <summary>Filters out anomalies in the provided data.</summary>
    Task<IReadOnlyList<TimeSeriesPointDto>> FilterAsync(AnomalyFilterRequestDto request, CancellationToken cancellationToken);
}
