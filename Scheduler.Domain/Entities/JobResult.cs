// PHASE 1: Scheduler.Domain/Entities/JobResult.cs
using System;
using System.Collections.Generic;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Domain.Entities;

/// <summary>Represents the result of a completed job.</summary>
public class JobResult
{
    /// <summary>Identifier of the job that produced the result.</summary>
    public JobId JobId { get; set; }

    /// <summary>Final status of the job.</summary>
    public JobStatus Status { get; set; }

    /// <summary>Summary of the result.</summary>
    public string? Summary { get; set; }

    /// <summary>Serialized payload for the result.</summary>
    public string? PayloadJson { get; set; }

    /// <summary>Errors encountered during processing.</summary>
    public List<JobError> Errors { get; set; }

    /// <summary>Summaries captured from task handlers.</summary>
    public List<string> TaskSummaries { get; set; }

    /// <summary>Timestamp the job completed.</summary>
    public DateTimeOffset CompletedAtUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobResult"/> class.</summary>
    public JobResult()
    {
        JobId = new JobId(Guid.Empty);
        Errors = new List<JobError>();
        TaskSummaries = new List<string>();
    }

    /// <summary>Initializes a new instance of the <see cref="JobResult"/> class with required fields.</summary>
    public JobResult(JobId jobId, JobStatus status)
    {
        JobId = jobId;
        Status = status;
        Errors = new List<JobError>();
        TaskSummaries = new List<string>();
    }
}
