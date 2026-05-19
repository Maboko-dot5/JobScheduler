// PHASE 2: Scheduler.Application/Interfaces/Services/IPhysicalFilterService.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines physical filtering operations.</summary>
public interface IPhysicalFilterService
{
    /// <summary>Filters out values outside physical limits.</summary>
    Task<IReadOnlyList<TimeSeriesPointDto>> FilterAsync(PhysicalFilterRequestDto request, CancellationToken cancellationToken);
}
