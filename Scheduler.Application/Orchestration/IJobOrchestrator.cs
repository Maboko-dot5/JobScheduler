// PHASE 3: Scheduler.Application/Orchestration/IJobOrchestrator.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Orchestration;

/// <summary>Defines job orchestration operations.</summary>
public interface IJobOrchestrator
{
    /// <summary>Submits a new job to the system.</summary>
    Task<JobSubmissionResponseDto> SubmitAsync(JobSubmissionRequestDto request, CancellationToken cancellationToken);

    /// <summary>Gets the status of a job.</summary>
    Task<JobStatusResponseDto?> GetStatusAsync(JobId jobId, CancellationToken cancellationToken);

    /// <summary>Lists all jobs.</summary>
    Task<IReadOnlyList<JobListItemDto>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Gets job details by identifier.</summary>
    Task<JobDetailsDto?> GetDetailsAsync(JobId jobId, CancellationToken cancellationToken);
}
