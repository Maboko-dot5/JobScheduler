// PHASE 11: Scheduler.Tests.Unit/Handlers/AnomalyFilterHandlerTests.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Scheduler.Application.Handlers;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Unit.Handlers;

/// <summary>Unit tests for <see cref="AnomalyFilterHandler"/>.</summary>
public class AnomalyFilterHandlerTests
{
    /// <summary>Ensures the handler returns a placeholder result.</summary>
    [Fact]
    public async Task ExecuteAsync_ReturnsPlaceholderResult()
    {
        var service = new Mock<IAnomalyFilterService>();
        var ruleProvider = new Mock<IFilteringRuleProvider>();
        var repo = new Mock<ITimeSeriesRepository>();
        var logger = new Mock<ILogger<AnomalyFilterHandler>>();
        ruleProvider
            .Setup(provider => provider.GetAnomalyRuleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Scheduler.Domain.ValueObjects.AnomalyFilterRule(3.0, 10));
        repo
            .Setup(r => r.GetSeriesAsync(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<System.DateTimeOffset>(), It.IsAny<System.DateTimeOffset>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSeriesPointDto>());
        service
            .Setup(s => s.FilterAsync(It.IsAny<AnomalyFilterRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSeriesPointDto>());
        var handler = new AnomalyFilterHandler(service.Object, ruleProvider.Object, repo.Object, logger.Object);
        var task = new JobContextDto(System.Guid.NewGuid(), TaskType.AnomalyFilter, "plant", new TimeRangeDto());
        task.Variables.Add("temperature");

        var result = await handler.ExecuteAsync(task, CancellationToken.None);

        Assert.NotNull(result);
    }
}
