// PHASE E: Scheduler.Tests.Unit/Infrastructure/FakeReportGeneratorTests.cs
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Enums;
using Scheduler.Infrastructure.Services;
using Xunit;

namespace Scheduler.Tests.Unit.Infrastructure;

/// <summary>Unit tests for <see cref="FakeReportGenerator"/>.</summary>
public class FakeReportGeneratorTests
{
    /// <summary>Ensures report generator returns a valid PDF payload.</summary>
    [Fact]
    public async Task GenerateAsync_ReturnsPdf()
    {
        var generator = new FakeReportGenerator();
        var request = new PdfReportRequestDto("series");
        request.Variables.Add("temperature");
        request.StatisticsSummaries.Add(CreateSummary(StatisticsWindow.Shift));
        request.StatisticsSummaries.Add(CreateSummary(StatisticsWindow.Daily));
        request.StatisticsSummaries.Add(CreateSummary(StatisticsWindow.Weekly));

        var report = await generator.GenerateAsync(request, CancellationToken.None);

        Assert.Equal("application/pdf", report.ContentType);
        Assert.NotEmpty(report.Content);
        var header = Encoding.ASCII.GetString(report.Content, 0, 5);
        Assert.Equal("%PDF-", header);
        var content = Encoding.ASCII.GetString(report.Content);
        Assert.Contains("Shift Statistics", content);
        Assert.Contains("Daily Statistics", content);
        Assert.Contains("Weekly Statistics", content);
        Assert.Contains("temperature", content);
        Assert.Contains("%%EOF", content);
    }

    private static ReportStatisticsSummaryDto CreateSummary(StatisticsWindow window)
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
        return new ReportStatisticsSummaryDto(window, statistics);
    }
}
