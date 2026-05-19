// PHASE A: Scheduler.Application/Dtos/JobContextDto.cs
using System;
using System.Collections.Generic;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents execution context for a job task.</summary>
public class JobContextDto
{
    /// <summary>Job identifier.</summary>
    public Guid JobId { get; set; }

    /// <summary>Task type.</summary>
    public TaskType TaskType { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variables to process.</summary>
    public List<string> Variables { get; set; }

    /// <summary>Time range for the task.</summary>
    public TimeRangeDto TimeRange { get; set; }

    /// <summary>Serialized parameters for the task.</summary>
    public string? ParametersJson { get; set; }

    /// <summary>Optional statistics window for statistics tasks.</summary>
    public StatisticsWindow? StatisticsWindow { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobContextDto"/> class.</summary>
    public JobContextDto()
    {
        JobId = Guid.Empty;
        PlantId = string.Empty;
        Variables = new List<string>();
        TimeRange = new TimeRangeDto();
    }

    /// <summary>Initializes a new instance of the <see cref="JobContextDto"/> class with required fields.</summary>
    public JobContextDto(Guid jobId, TaskType taskType, string plantId, TimeRangeDto timeRange)
    {
        JobId = jobId;
        TaskType = taskType;
        PlantId = plantId;
        TimeRange = timeRange;
        Variables = new List<string>();
    }
}
