// PHASE E: Scheduler.Tests.Unit/Infrastructure/FakeReportGeneratorTests.cs
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
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

        var report = await generator.GenerateAsync(request, CancellationToken.None);

        Assert.Equal("application/pdf", report.ContentType);
        Assert.NotEmpty(report.Content);
        var header = Encoding.ASCII.GetString(report.Content, 0, 5);
        Assert.Equal("%PDF-", header);
        Assert.Contains("%%EOF", Encoding.ASCII.GetString(report.Content));
    }
}
