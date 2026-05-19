// PHASE 2: Scheduler.Application/Dtos/JobSubmissionRequestDto.cs
using System.Collections.Generic;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a job submission request.</summary>
public class JobSubmissionRequestDto
{
    /// <summary>Client-supplied job identifier.</summary>
    public System.Guid JobId { get; set; }

    /// <summary>Job name.</summary>
    public string Name { get; set; }

    /// <summary>Optional job description.</summary>
    public string? Description { get; set; }

    /// <summary>Primary task type.</summary>
    public TaskType PrimaryTaskType { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variables to process.</summary>
    public List<string> Variables { get; set; }

    /// <summary>Requested time range.</summary>
    public TimeRangeDto RequestedTimeRange { get; set; }

    /// <summary>Client identity that requested the job.</summary>
    public string RequestedBy { get; set; }

    /// <summary>Optional correlation identifier.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Task list for the job.</summary>
    public List<JobTaskDto> Tasks { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobSubmissionRequestDto"/> class.</summary>
    public JobSubmissionRequestDto()
    {
        JobId = System.Guid.Empty;
        Name = string.Empty;
        PlantId = string.Empty;
        Variables = new List<string>();
        RequestedTimeRange = new TimeRangeDto();
        RequestedBy = string.Empty;
        Tasks = new List<JobTaskDto>();
    }

    /// <summary>Initializes a new instance of the <see cref="JobSubmissionRequestDto"/> class with required fields.</summary>
    public JobSubmissionRequestDto(string name, TaskType primaryTaskType, string requestedBy)
    {
        JobId = System.Guid.Empty;
        Name = name;
        PrimaryTaskType = primaryTaskType;
        PlantId = string.Empty;
        Variables = new List<string>();
        RequestedTimeRange = new TimeRangeDto();
        RequestedBy = requestedBy;
        Tasks = new List<JobTaskDto>();
    }
}
