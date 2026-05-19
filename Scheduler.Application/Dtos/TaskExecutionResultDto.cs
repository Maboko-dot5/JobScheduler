// PHASE 2: Scheduler.Application/Dtos/TaskExecutionResultDto.cs
using System.Collections.Generic;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Dtos;

/// <summary>Represents the outcome of a task execution.</summary>
public class TaskExecutionResultDto
{
    /// <summary>Indicates whether the task succeeded.</summary>
    public bool Succeeded { get; set; }

    /// <summary>Optional output location or identifier.</summary>
    public string? OutputLocation { get; set; }

    /// <summary>Optional summary.</summary>
    public string? Summary { get; set; }

    /// <summary>Errors encountered during task execution.</summary>
    public List<JobError> Errors { get; set; }

    /// <summary>Initializes a new instance of the <see cref="TaskExecutionResultDto"/> class.</summary>
    public TaskExecutionResultDto()
    {
        Errors = new List<JobError>();
    }

    /// <summary>Initializes a new instance of the <see cref="TaskExecutionResultDto"/> class with required fields.</summary>
    public TaskExecutionResultDto(bool succeeded)
    {
        Succeeded = succeeded;
        Errors = new List<JobError>();
    }
}
