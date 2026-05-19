// PHASE 2: Scheduler.Application/Dtos/JobDetailsDto.cs
using System.Collections.Generic;

namespace Scheduler.Application.Dtos;

/// <summary>Represents detailed information for a job.</summary>
public class JobDetailsDto
{
    /// <summary>Job status response.</summary>
    public JobStatusResponseDto Status { get; set; }

    /// <summary>Task list associated with the job.</summary>
    public List<JobTaskDto> Tasks { get; set; }

    /// <summary>Result of the job.</summary>
    public JobResultDto? Result { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobDetailsDto"/> class.</summary>
    public JobDetailsDto()
    {
        Status = new JobStatusResponseDto();
        Tasks = new List<JobTaskDto>();
    }

    /// <summary>Initializes a new instance of the <see cref="JobDetailsDto"/> class with required fields.</summary>
    public JobDetailsDto(JobStatusResponseDto status)
    {
        Status = status;
        Tasks = new List<JobTaskDto>();
    }
}
