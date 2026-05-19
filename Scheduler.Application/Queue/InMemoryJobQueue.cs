// PHASE 3: Scheduler.Application/Queue/InMemoryJobQueue.cs
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Interfaces.Queue;
using Scheduler.Domain.Entities;

namespace Scheduler.Application.Queue;

/// <summary>In-memory implementation of a job queue.</summary>
public class InMemoryJobQueue : IJobQueue
{
    private readonly ConcurrentQueue<Job> _queue;

    /// <summary>Initializes a new instance of the <see cref="InMemoryJobQueue"/> class.</summary>
    public InMemoryJobQueue()
    {
        _queue = new ConcurrentQueue<Job>();
    }

    /// <inheritdoc />
    public Task EnqueueAsync(Job job, CancellationToken cancellationToken)
    {
        _queue.Enqueue(job);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Job?> DequeueAsync(CancellationToken cancellationToken)
    {
        _queue.TryDequeue(out var job);
        return Task.FromResult(job);
    }
}
