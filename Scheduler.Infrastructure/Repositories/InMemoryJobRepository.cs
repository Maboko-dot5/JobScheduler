// PHASE 7: Scheduler.Infrastructure/Repositories/InMemoryJobRepository.cs
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Domain.Entities;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Infrastructure.Repositories;

/// <summary>Stores jobs in memory.</summary>
public class InMemoryJobRepository : IJobRepository
{
    private readonly ConcurrentDictionary<string, Job> _jobs;

    /// <summary>Initializes a new instance of the <see cref="InMemoryJobRepository"/> class.</summary>
    public InMemoryJobRepository()
    {
        _jobs = new ConcurrentDictionary<string, Job>();
    }

    /// <inheritdoc />
    public Task AddAsync(Job job, CancellationToken cancellationToken)
    {
        _jobs[job.Id.Value.ToString()] = job;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Job?> GetByIdAsync(JobId jobId, CancellationToken cancellationToken)
    {
        _jobs.TryGetValue(jobId.Value.ToString(), out var job);
        return Task.FromResult(job);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Job>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<Job> result = new List<Job>(_jobs.Values);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task UpdateAsync(Job job, CancellationToken cancellationToken)
    {
        _jobs[job.Id.Value.ToString()] = job;
        return Task.CompletedTask;
    }
}
