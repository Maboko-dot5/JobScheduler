using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Stores generated report documents for later retrieval.</summary>
public interface IReportStore
{
    /// <summary>Saves a generated report for a job.</summary>
    Task SaveAsync(JobId jobId, ReportDocumentDto report, CancellationToken cancellationToken);

    /// <summary>Gets the generated report for a job.</summary>
    Task<ReportDocumentDto?> GetAsync(JobId jobId, CancellationToken cancellationToken);
}
