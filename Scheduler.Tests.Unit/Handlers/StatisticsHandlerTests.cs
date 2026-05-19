// PHASE 11: Scheduler.Tests.Unit/Handlers/StatisticsHandlerTests.cs
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
    /// <summary>Ensures the handler returns a placeholder result.</summary>
    [Fact]
    public async Task ExecuteAsync_ReturnsPlaceholderResult()
    {
        var service = new Mock<IStatisticsService>();
        var repo = new Mock<ITimeSeriesRepository>();
        var logger = new Mock<ILogger<StatisticsHandler>>();
        repo
            .Setup(r => r.GetSeriesAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<System.DateTimeOffset>(), It.IsAny<System.DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSeriesPointDto>());
        service
            .Setup(s => s.CalculateAsync(It.IsAny<StatisticsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StatisticsResultDto("series"));
        var handler = new StatisticsHandler(service.Object, repo.Object, logger.Object);
        var task = new JobContextDto(System.Guid.NewGuid(), TaskType.Statistics, "plant", new TimeRangeDto());
        task.Variables.Add("temperature");

        var result = await handler.ExecuteAsync(task, CancellationToken.None);

        Assert.NotNull(result);
    }
}
