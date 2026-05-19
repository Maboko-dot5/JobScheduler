// PHASE E: Scheduler.Tests.Unit/Infrastructure/MockTimeSeriesRepositoryTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Infrastructure.Repositories;
using Xunit;

namespace Scheduler.Tests.Unit.Infrastructure;

/// <summary>Unit tests for <see cref="MockTimeSeriesRepository"/>.</summary>
public class MockTimeSeriesRepositoryTests
{
    /// <summary>Ensures repository returns points for variables within the time range.</summary>
    [Fact]
    public async Task GetSeriesAsync_ReturnsPointsInRange()
    {
        var repo = new MockTimeSeriesRepository();
        var start = new DateTimeOffset(2026, 5, 18, 0, 0, 0, TimeSpan.Zero);
        var end = start.AddMinutes(5);
        var variables = new List<string> { "temperature", "pressure" };

        var points = await repo.GetSeriesAsync("plant-001", variables, start, end, CancellationToken.None);

        Assert.Equal(12, points.Count);
        Assert.All(points, point => Assert.Contains(point.Variable, variables));
        Assert.All(points, point => Assert.InRange(point.TimestampUtc, start, end));
    }
}
