// PHASE 7: Scheduler.Infrastructure/Services/FakePhysicalFilterService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Provides fake physical filtering results.</summary>
public class FakePhysicalFilterService : IPhysicalFilterService
{
    /// <inheritdoc />
    public Task<IReadOnlyList<TimeSeriesPointDto>> FilterAsync(PhysicalFilterRequestDto request, CancellationToken cancellationToken)
    {
        if (request.Rule is null)
        {
            return Task.FromResult<IReadOnlyList<TimeSeriesPointDto>>(request.Points);
        }

        var filtered = request.Points
            .Where(point => point.Value >= request.Rule.MinValue && point.Value <= request.Rule.MaxValue)
            .ToList();

        return Task.FromResult<IReadOnlyList<TimeSeriesPointDto>>(filtered);
    }
}
