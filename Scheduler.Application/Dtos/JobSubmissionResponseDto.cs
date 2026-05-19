// PHASE 2: Scheduler.Application/Dtos/JobSubmissionResponseDto.cs
using System;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a job submission response.</summary>
public class JobSubmissionResponseDto
{
    /// <summary>Job identifier.</summary>
    public Guid JobId { get; set; }

    /// <summary>Current status of the job.</summary>
    public JobStatus Status { get; set; }

    /// <summary>Status URL for polling.</summary>
    public string? StatusUrl { get; set; }

    /// <summary>Optional response message.</summary>
    public string? Message { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobSubmissionResponseDto"/> class.</summary>
    public JobSubmissionResponseDto()
    {
        JobId = Guid.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="JobSubmissionResponseDto"/> class with required fields.</summary>
    public JobSubmissionResponseDto(Guid jobId, JobStatus status)
    {
        JobId = jobId;
        Status = status;
    }
}
