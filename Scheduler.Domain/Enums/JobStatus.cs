// PHASE 1: Scheduler.Domain/Enums/JobStatus.cs
namespace Scheduler.Domain.Enums;

/// <summary>Represents the lifecycle status of a job.</summary>
public enum JobStatus
{
    /// <summary>Job has been created but not queued.</summary>
    Pending = 0,

    /// <summary>Job is queued for processing.</summary>
    Queued = 1,

    /// <summary>Job is currently running.</summary>
    Running = 2,

    /// <summary>Job completed successfully.</summary>
    Completed = 3,

    /// <summary>Job failed during processing.</summary>
    Failed = 4,

    /// <summary>Job was cancelled.</summary>
    Cancelled = 5
}
