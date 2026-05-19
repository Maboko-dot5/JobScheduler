// PHASE 2: Scheduler.Application/Interfaces/Repositories/IJobRepository.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Domain.Entities;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Interfaces.Repositories;

/// <summary>Defines persistence operations for jobs.</summary>
public interface IJobRepository
{
    /// <summary>Adds a new job.</summary>
    Task AddAsync(Job job, CancellationToken cancellationToken);

    /// <summary>Gets a job by identifier.</summary>
    Task<Job?> GetByIdAsync(JobId jobId, CancellationToken cancellationToken);

    /// <summary>Lists all jobs.</summary>
    Task<IReadOnlyList<Job>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Updates an existing job.</summary>
    Task UpdateAsync(Job job, CancellationToken cancellationToken);
}
