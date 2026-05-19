// PHASE 3: Scheduler.Application/Interfaces/Queue/IJobQueue.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Domain.Entities;

namespace Scheduler.Application.Interfaces.Queue;

/// <summary>Defines a queue for job processing.</summary>
public interface IJobQueue
{
    /// <summary>Enqueues a job for processing.</summary>
    Task EnqueueAsync(Job job, CancellationToken cancellationToken);

    /// <summary>Dequeues the next job for processing.</summary>
    Task<Job?> DequeueAsync(CancellationToken cancellationToken);
}
