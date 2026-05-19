// PHASE 1: Scheduler.Domain/Entities/JobTask.cs
using System;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Domain.Entities;

/// <summary>Represents a task within a job.</summary>
public class JobTask
{
    /// <summary>Unique task identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Identifier of the job that owns the task.</summary>
    public JobId JobId { get; set; }

    /// <summary>Task type to execute.</summary>
    public TaskType TaskType { get; set; }

    /// <summary>Task status.</summary>
    public JobTaskStatus Status { get; set; }

    /// <summary>Optional time range for the task.</summary>
    public TimeRange? TimeRange { get; set; }

    /// <summary>Optional statistics window for statistics tasks.</summary>
    public StatisticsWindow? StatisticsWindow { get; set; }

    /// <summary>Serialized parameters for the task.</summary>
    public string? ParametersJson { get; set; }

    /// <summary>Optional output location or identifier.</summary>
    public string? OutputLocation { get; set; }

    /// <summary>Timestamp the task was created.</summary>
    public DateTimeOffset CreatedAtUtc { get; set; }

    /// <summary>Timestamp the task started.</summary>
    public DateTimeOffset? StartedAtUtc { get; set; }

    /// <summary>Timestamp the task completed.</summary>
    public DateTimeOffset? CompletedAtUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobTask"/> class.</summary>
    public JobTask()
    {
        Id = Guid.Empty;
        JobId = new JobId(Guid.Empty);
        Status = JobTaskStatus.Pending;
    }

    /// <summary>Initializes a new instance of the <see cref="JobTask"/> class with required fields.</summary>
    public JobTask(Guid id, JobId jobId, TaskType taskType)
    {
        Id = id;
        JobId = jobId;
        TaskType = taskType;
        Status = JobTaskStatus.Pending;
    }
}
