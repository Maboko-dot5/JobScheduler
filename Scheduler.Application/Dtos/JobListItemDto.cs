// PHASE 2: Scheduler.Application/Dtos/JobListItemDto.cs
using System;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a summary list item for a job.</summary>
public class JobListItemDto
{
    /// <summary>Job identifier.</summary>
    public Guid JobId { get; set; }

    /// <summary>Job name.</summary>
    public string Name { get; set; }

    /// <summary>Current job status.</summary>
    public JobStatus Status { get; set; }

    /// <summary>Timestamp the job was created.</summary>
    public DateTimeOffset CreatedAtUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobListItemDto"/> class.</summary>
    public JobListItemDto()
    {
        JobId = Guid.Empty;
        Name = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="JobListItemDto"/> class with required fields.</summary>
    public JobListItemDto(Guid jobId, string name, JobStatus status)
    {
        JobId = jobId;
        Name = name;
        Status = status;
    }
}
