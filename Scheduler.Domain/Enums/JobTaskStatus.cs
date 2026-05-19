// PHASE 1: Scheduler.Domain/Enums/JobTaskStatus.cs
namespace Scheduler.Domain.Enums;

/// <summary>Represents the lifecycle status of a job task.</summary>
public enum JobTaskStatus
{
    /// <summary>Task has been created but not started.</summary>
    Pending = 0,

    /// <summary>Task is currently running.</summary>
    Running = 1,

    /// <summary>Task completed successfully.</summary>
    Completed = 2,

    /// <summary>Task failed during processing.</summary>
    Failed = 3
}
