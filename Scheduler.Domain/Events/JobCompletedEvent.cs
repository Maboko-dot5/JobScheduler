// PHASE 1: Scheduler.Domain/Events/JobCompletedEvent.cs
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Domain.Events;

/// <summary>Event raised when a job completes.</summary>
public class JobCompletedEvent : DomainEvent
{
    /// <summary>Identifier of the completed job.</summary>
    public JobId JobId { get; set; }

    /// <summary>Final job status.</summary>
    public JobStatus Status { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobCompletedEvent"/> class.</summary>
    public JobCompletedEvent()
    {
        JobId = new JobId();
        Status = JobStatus.Completed;
    }

    /// <summary>Initializes a new instance of the <see cref="JobCompletedEvent"/> class with required fields.</summary>
    public JobCompletedEvent(JobId jobId, JobStatus status)
    {
        JobId = jobId;
        Status = status;
    }
}
