using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Infrastructure.Services;

/// <summary>In-memory store for generated reports.</summary>
public class InMemoryReportStore : IReportStore
{
    private readonly ConcurrentDictionary<string, ReportDocumentDto> _reports = new();

    /// <inheritdoc />
    public Task SaveAsync(JobId jobId, ReportDocumentDto report, CancellationToken cancellationToken)
    {
        _reports[jobId.Value.ToString()] = report;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<ReportDocumentDto?> GetAsync(JobId jobId, CancellationToken cancellationToken)
    {
        _reports.TryGetValue(jobId.Value.ToString(), out var report);
        return Task.FromResult(report);
    }
}
