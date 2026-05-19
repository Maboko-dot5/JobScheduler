// PHASE 11: Scheduler.Tests.Unit/Handlers/PdfReportHandlerTests.cs
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Scheduler.Application.Handlers;
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
        var email = new Mock<IEmailService>();
        var logger = new Mock<ILogger<PdfReportHandler>>();
        report
            .Setup(r => r.GenerateAsync(It.IsAny<PdfReportRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReportDocumentDto("report.pdf", "application/pdf", System.Array.Empty<byte>()));
        var handler = new PdfReportHandler(report.Object, email.Object, logger.Object);
        var task = new JobContextDto(System.Guid.NewGuid(), TaskType.PdfReport, "plant", new TimeRangeDto());
        task.Variables.Add("temperature");

        var result = await handler.ExecuteAsync(task, CancellationToken.None);

        Assert.NotNull(result);
    }
}
