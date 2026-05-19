// PHASE 7: Scheduler.Infrastructure/Services/FakeReportGenerator.cs
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Provides fake PDF report generation.</summary>
public class FakeReportGenerator : IReportGenerator
{
    /// <inheritdoc />
    public Task<ReportDocumentDto> GenerateAsync(PdfReportRequestDto request, CancellationToken cancellationToken)
    {
        var content = Encoding.UTF8.GetBytes("Fake PDF content");
        return Task.FromResult(new ReportDocumentDto("report.pdf", "application/pdf", content));
    }
}
