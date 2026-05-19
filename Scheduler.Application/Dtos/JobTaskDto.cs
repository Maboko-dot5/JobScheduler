// PHASE 2: Scheduler.Application/Dtos/JobTaskDto.cs
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a task definition for a job submission.</summary>
public class JobTaskDto
{
    /// <summary>Task type.</summary>
    public TaskType TaskType { get; set; }

    /// <summary>Optional time range for the task.</summary>
    public TimeRangeDto? TimeRange { get; set; }

    /// <summary>Serialized parameters for the task.</summary>
    public string? ParametersJson { get; set; }

    /// <summary>Optional statistics window for statistics tasks.</summary>
    public StatisticsWindow? StatisticsWindow { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobTaskDto"/> class.</summary>
    public JobTaskDto()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="JobTaskDto"/> class with required fields.</summary>
    public JobTaskDto(TaskType taskType)
    {
        TaskType = taskType;
    }
}
