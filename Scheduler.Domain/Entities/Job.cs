// PHASE 1: Scheduler.Domain/Entities/Job.cs
using System;
using System.Collections.Generic;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Domain.Entities;

/// <summary>Represents a job submitted to the scheduler.</summary>
public class Job
{
    /// <summary>Unique job identifier.</summary>
    public JobId Id { get; set; }

    /// <summary>Human-friendly name for the job.</summary>
    public string Name { get; set; }

    /// <summary>Optional job description.</summary>
    public string? Description { get; set; }

    /// <summary>Current job status.</summary>
    public JobStatus Status { get; set; }

    /// <summary>Priority assigned to the job.</summary>
    public JobPriority Priority { get; set; }

    /// <summary>Primary task type for the job.</summary>
    public TaskType PrimaryTaskType { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variables to process.</summary>
    public List<string> Variables { get; set; }

    /// <summary>Requested time range for processing.</summary>
    public TimeRange? RequestedTimeRange { get; set; }

    /// <summary>Timestamp the job was created.</summary>
    public DateTimeOffset CreatedAtUtc { get; set; }

    /// <summary>Timestamp the job was last updated.</summary>
    public DateTimeOffset UpdatedAtUtc { get; set; }

    /// <summary>Timestamp the job was scheduled.</summary>
    public DateTimeOffset? ScheduledAtUtc { get; set; }

    /// <summary>Timestamp the job started.</summary>
    public DateTimeOffset? StartedAtUtc { get; set; }

    /// <summary>Timestamp the job completed.</summary>
    public DateTimeOffset? CompletedAtUtc { get; set; }

    /// <summary>Client identity that requested the job.</summary>
    public string RequestedBy { get; set; }

    /// <summary>Optional correlation identifier for tracing.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Tasks associated with the job.</summary>
    public List<JobTask> Tasks { get; set; }

    /// <summary>Result of the job once completed.</summary>
    public JobResult? Result { get; set; }

    /// <summary>Initializes a new instance of the <see cref="Job"/> class.</summary>
    public Job()
    {
        Id = new JobId(Guid.Empty);
        Name = string.Empty;
        PlantId = string.Empty;
        Variables = new List<string>();
        RequestedBy = string.Empty;
        Tasks = new List<JobTask>();
        Status = JobStatus.Pending;
        Priority = JobPriority.Normal;
    }

    /// <summary>Initializes a new instance of the <see cref="Job"/> class with required fields.</summary>
    public Job(JobId id, string name, TaskType primaryTaskType, string requestedBy)
    {
        Id = id;
        Name = name;
        PrimaryTaskType = primaryTaskType;
        PlantId = string.Empty;
        Variables = new List<string>();
        RequestedBy = requestedBy;
        Tasks = new List<JobTask>();
        Status = JobStatus.Pending;
        Priority = JobPriority.Normal;
    }
}
