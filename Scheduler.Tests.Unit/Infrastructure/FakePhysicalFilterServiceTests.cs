// PHASE E: Scheduler.Tests.Unit/Infrastructure/FakePhysicalFilterServiceTests.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.ValueObjects;
using Scheduler.Infrastructure.Services;
using Xunit;

namespace Scheduler.Tests.Unit.Infrastructure;

/// <summary>Unit tests for <see cref="FakePhysicalFilterService"/>.</summary>
public class FakePhysicalFilterServiceTests
{
    /// <summary>Ensures values outside the rule are filtered.</summary>
    [Fact]
    public async Task FilterAsync_RemovesOutOfRangePoints()
    {
        var service = new FakePhysicalFilterService();
        var request = new PhysicalFilterRequestDto("series")
        {
            Rule = new PhysicalFilterRule(0, 100),
            Points = new List<TimeSeriesPointDto>
            {
                new TimeSeriesPointDto(System.DateTimeOffset.UtcNow, "temperature", 0),
                new TimeSeriesPointDto(System.DateTimeOffset.UtcNow, "temperature", 50),
                new TimeSeriesPointDto(System.DateTimeOffset.UtcNow, "temperature", 200)
            }
        };

        var filtered = await service.FilterAsync(request, CancellationToken.None);

        Assert.Equal(2, filtered.Count);
    }
}
