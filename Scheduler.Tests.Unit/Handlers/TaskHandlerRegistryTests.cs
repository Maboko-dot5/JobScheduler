// PHASE D: Scheduler.Tests.Unit/Handlers/TaskHandlerRegistryTests.cs
using Microsoft.Extensions.Logging;
using Moq;
using Scheduler.Application.Handlers;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Unit.Handlers;

/// <summary>Unit tests for <see cref="TaskHandlerRegistry"/>.</summary>
public class TaskHandlerRegistryTests
{
    /// <summary>Ensures registry resolves handlers by task type.</summary>
    [Fact]
    public void GetHandler_ReturnsHandlersForEachTaskType()
    {
        var physical = new PhysicalFilterHandler(
            new Mock<IPhysicalFilterService>().Object,
            new Mock<IFilteringRuleProvider>().Object,
            new Mock<ITimeSeriesRepository>().Object,
            new Mock<ILogger<PhysicalFilterHandler>>().Object);
        var anomaly = new AnomalyFilterHandler(
            new Mock<IAnomalyFilterService>().Object,
            new Mock<IFilteringRuleProvider>().Object,
            new Mock<ITimeSeriesRepository>().Object,
            new Mock<ILogger<AnomalyFilterHandler>>().Object);
        var stats = new StatisticsHandler(
            new Mock<IStatisticsService>().Object,
            new Mock<ITimeSeriesRepository>().Object,
            new Mock<ILogger<StatisticsHandler>>().Object);
        var pdf = new PdfReportHandler(new Mock<IReportGenerator>().Object, new Mock<IEmailService>().Object, new Mock<ILogger<PdfReportHandler>>().Object);

        ITaskHandlerRegistry registry = new TaskHandlerRegistry(new ITaskHandler[] { physical, anomaly, stats, pdf });

        Assert.NotNull(registry.GetHandler(TaskType.PhysicalFilter));
        Assert.NotNull(registry.GetHandler(TaskType.AnomalyFilter));
        Assert.NotNull(registry.GetHandler(TaskType.Statistics));
        Assert.NotNull(registry.GetHandler(TaskType.PdfReport));
    }
}
