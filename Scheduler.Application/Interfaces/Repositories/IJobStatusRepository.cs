// PHASE 2: Scheduler.Application/Interfaces/Repositories/IJobStatusRepository.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Interfaces.Repositories;

/// <summary>Defines persistence operations for job status updates.</summary>
public interface IJobStatusRepository
{
    /// <summary>Gets a job status by identifier.</summary>
    Task<JobStatus?> GetStatusAsync(JobId jobId, CancellationToken cancellationToken);

    /// <summary>Updates a job status.</summary>
    Task UpdateStatusAsync(JobId jobId, JobStatus status, CancellationToken cancellationToken);
}
