// PHASE 2: Scheduler.Application/Interfaces/Services/IReportGenerator.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines PDF report generation operations.</summary>
public interface IReportGenerator
{
    /// <summary>Generates a PDF report for the request.</summary>
    Task<ReportDocumentDto> GenerateAsync(PdfReportRequestDto request, CancellationToken cancellationToken);
}
