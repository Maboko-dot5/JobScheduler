// PHASE 1: Scheduler.Domain/Events/JobSubmittedEvent.cs
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Domain.Events;

/// <summary>Event raised when a job is submitted.</summary>
public class JobSubmittedEvent : DomainEvent
{
    /// <summary>Identifier of the submitted job.</summary>
    public JobId JobId { get; set; }

    /// <summary>Client identity that submitted the job.</summary>
    public string SubmittedBy { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobSubmittedEvent"/> class.</summary>
    public JobSubmittedEvent()
    {
        JobId = new JobId();
        SubmittedBy = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="JobSubmittedEvent"/> class with required fields.</summary>
    public JobSubmittedEvent(JobId jobId, string submittedBy)
    {
        JobId = jobId;
        SubmittedBy = submittedBy;
    }
}
