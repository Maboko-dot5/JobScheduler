// PHASE 2: Scheduler.Application/Dtos/JobStatusResponseDto.cs
using System;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a job status response.</summary>
public class JobStatusResponseDto
{
    /// <summary>Job identifier.</summary>
    public Guid JobId { get; set; }

    /// <summary>Current job status.</summary>
    public JobStatus Status { get; set; }

    /// <summary>Optional summary message.</summary>
    public string? Summary { get; set; }

    /// <summary>Job result details.</summary>
    public JobResultDto? Result { get; set; }

    /// <summary>Timestamp the job was last updated.</summary>
    public DateTimeOffset? UpdatedAtUtc { get; set; }

    /// <summary>Timestamp the job completed.</summary>
    public DateTimeOffset? CompletedAtUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobStatusResponseDto"/> class.</summary>
    public JobStatusResponseDto()
    {
        JobId = Guid.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="JobStatusResponseDto"/> class with required fields.</summary>
    public JobStatusResponseDto(Guid jobId, JobStatus status)
    {
        JobId = jobId;
        Status = status;
    }
}
