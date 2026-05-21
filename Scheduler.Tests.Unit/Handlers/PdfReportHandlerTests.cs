// PHASE 11: Scheduler.Tests.Unit/Handlers/PdfReportHandlerTests.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Scheduler.Application.Handlers;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Unit.Handlers;

/// <summary>Unit tests for <see cref="PdfReportHandler"/>.</summary>
public class PdfReportHandlerTests
{
    /// <summary>Ensures the handler returns a placeholder result.</summary>
    [Fact]
    public async Task ExecuteAsync_ReturnsPlaceholderResult()
    {
        var report = new Mock<IReportGenerator>();
        var reportStore = new Mock<IReportStore>();
        var outbox = new Mock<IEmailOutbox>();
        var statistics = new Mock<IStatisticsService>();
        var timeSeries = new Mock<ITimeSeriesRepository>();
        var logger = new Mock<ILogger<PdfReportHandler>>();
        PdfReportRequestDto? capturedRequest = null;
        timeSeries
            .Setup(r => r.GetSeriesAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<System.DateTimeOffset>(),
                It.IsAny<System.DateTimeOffset>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TimeSeriesPointDto>
            {
                new TimeSeriesPointDto(System.DateTimeOffset.UtcNow, "temperature", 50)
            });
        statistics
            .Setup(s => s.CalculateAsync(It.IsAny<StatisticsRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StatisticsRequestDto request, CancellationToken _) => CreateStatistics(request.Window));
        report
            .Setup(r => r.GenerateAsync(It.IsAny<PdfReportRequestDto>(), It.IsAny<CancellationToken>()))
            .Callback<PdfReportRequestDto, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new ReportDocumentDto("report.pdf", "application/pdf", System.Array.Empty<byte>()));
        var handler = new PdfReportHandler(
            report.Object,
            reportStore.Object,
            outbox.Object,
            statistics.Object,
            timeSeries.Object,
            logger.Object);
        var task = new JobContextDto(System.Guid.NewGuid(), TaskType.PdfReport, "plant", new TimeRangeDto());
        task.Variables.Add("temperature");

        var result = await handler.ExecuteAsync(task, CancellationToken.None);

        Assert.NotNull(result);
        reportStore.Verify(
            s => s.SaveAsync(
                It.Is<Scheduler.Domain.ValueObjects.JobId>(id => id.Value == task.JobId),
                It.IsAny<ReportDocumentDto>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        outbox.Verify(o => o.EnqueueAsync(It.IsAny<EmailDeliveryDto>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedRequest);
        Assert.Collection(
            capturedRequest!.StatisticsSummaries,
            item => Assert.Equal(StatisticsWindow.Shift, item.Window),
            item => Assert.Equal(StatisticsWindow.Daily, item.Window),
            item => Assert.Equal(StatisticsWindow.Weekly, item.Window));
        Assert.NotNull(capturedRequest.Statistics);
        Assert.Contains("Shift/Daily/Weekly statistics included", result.Summary);
    }

    private static StatisticsResultDto CreateStatistics(StatisticsWindow window)
    {
        var statistics = new StatisticsResultDto(window.ToString());
        statistics.Metrics["count"] = 1;
        statistics.Metrics["min"] = 50;
        statistics.Metrics["max"] = 50;
        statistics.Metrics["avg"] = 50;
        statistics.Metrics["temperature.count"] = 1;
        statistics.Metrics["temperature.min"] = 50;
        statistics.Metrics["temperature.max"] = 50;
        statistics.Metrics["temperature.avg"] = 50;
        statistics.Metrics["uptimePct"] = 100;
        statistics.Metrics["inTargetPct"] = 100;
        statistics.Metrics["windowHours"] = window == StatisticsWindow.Shift ? 8 : window == StatisticsWindow.Daily ? 24 : 168;
        statistics.Metrics["windowCount"] = 1;
        return statistics;
    }
}
