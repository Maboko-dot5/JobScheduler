// PHASE 2: Scheduler.Application/Dtos/JobResultDto.cs
using System;
using System.Collections.Generic;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Dtos;

/// <summary>Represents job result details.</summary>
public class JobResultDto
{
    /// <summary>Job identifier.</summary>
    public Guid JobId { get; set; }

    /// <summary>Final job status.</summary>
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

    /// <summary>Initializes a new instance of the <see cref="JobResultDto"/> class.</summary>
    public JobResultDto()
    {
        JobId = Guid.Empty;
        Errors = new List<JobError>();
        TaskSummaries = new List<string>();
    }

    /// <summary>Initializes a new instance of the <see cref="JobResultDto"/> class with required fields.</summary>
    public JobResultDto(Guid jobId, JobStatus status)
    {
        JobId = jobId;
        Status = status;
        Errors = new List<JobError>();
        TaskSummaries = new List<string>();
    }
}
