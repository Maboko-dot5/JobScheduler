// PHASE 11: Scheduler.Tests.Unit/Handlers/StatisticsHandlerTests.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Scheduler.Application.Handlers;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Unit.Handlers;

/// <summary>Unit tests for <see cref="StatisticsHandler"/>.</summary>
public class StatisticsHandlerTests
{
    /// <summary>Ensures the handler returns a readable summary with the calculated values.</summary>
    [Fact]
    public async Task ExecuteAsync_ReturnsCalculatedStatisticsSummary()
    {
        var service = new Mock<IStatisticsService>();
        var repo = new Mock<ITimeSeriesRepository>();
        var logger = new Mock<ILogger<StatisticsHandler>>();
        repo
            .Setup(r => r.GetSeriesAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<System.DateTimeOffset>(), It.IsAny<System.DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSeriesPointDto>());
        service
            .Setup(s => s.CalculateAsync(It.IsAny<StatisticsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateStatisticsResult());
        var handler = new StatisticsHandler(service.Object, repo.Object, logger.Object);
        var task = new JobContextDto(Guid.NewGuid(), TaskType.Statistics, "plant", new TimeRangeDto())
        {
            StatisticsWindow = StatisticsWindow.Daily
        };
        task.Variables.Add("temperature");

        var result = await handler.ExecuteAsync(task, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.Succeeded);
        Assert.Contains("Daily statistics", result.Summary);
        Assert.Contains("count=12", result.Summary);
        Assert.Contains("min=49.5", result.Summary);
        Assert.Contains("max=60.25", result.Summary);
        Assert.Contains("avg=55.125", result.Summary);
        Assert.Contains("uptime=98.5%", result.Summary);
        Assert.Contains("in target=97.25%", result.Summary);
        Assert.Contains("temperature: count=12", result.Summary);
        Assert.Contains("avg=55.125", result.Summary);
    }

    private static StatisticsResultDto CreateStatisticsResult()
    {
        var statistics = new StatisticsResultDto("series");
        statistics.Metrics["count"] = 12;
        statistics.Metrics["min"] = 49.5;
        statistics.Metrics["max"] = 60.25;
        statistics.Metrics["avg"] = 55.125;
        statistics.Metrics["uptimePct"] = 98.5;
        statistics.Metrics["inTargetPct"] = 97.25;
        statistics.Metrics["windowHours"] = 24;
        statistics.Metrics["windowCount"] = 1;
        statistics.Metrics["temperature.count"] = 12;
        statistics.Metrics["temperature.min"] = 49.5;
        statistics.Metrics["temperature.max"] = 60.25;
        statistics.Metrics["temperature.avg"] = 55.125;
        return statistics;
    }
}
