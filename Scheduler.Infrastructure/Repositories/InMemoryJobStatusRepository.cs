// PHASE 7: Scheduler.Infrastructure/Repositories/InMemoryJobStatusRepository.cs
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Infrastructure.Repositories;

/// <summary>Stores job statuses in memory.</summary>
public class InMemoryJobStatusRepository : IJobStatusRepository
{
    private readonly ConcurrentDictionary<string, JobStatus> _statusStore;

    /// <summary>Initializes a new instance of the <see cref="InMemoryJobStatusRepository"/> class.</summary>
    public InMemoryJobStatusRepository()
    {
        _statusStore = new ConcurrentDictionary<string, JobStatus>();
    }

    /// <inheritdoc />
    public Task<JobStatus?> GetStatusAsync(JobId jobId, CancellationToken cancellationToken)
    {
        return Task.FromResult(_statusStore.TryGetValue(jobId.Value.ToString(), out var status)
            ? status
            : (JobStatus?)null);
    }

    /// <inheritdoc />
    public Task UpdateStatusAsync(JobId jobId, JobStatus status, CancellationToken cancellationToken)
    {
        _statusStore[jobId.Value.ToString()] = status;
        return Task.CompletedTask;
    }
}
