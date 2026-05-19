// PHASE 1: Scheduler.Domain/Enums/JobPriority.cs
namespace Scheduler.Domain.Enums;

/// <summary>Represents job priority levels.</summary>
public enum JobPriority
{
    /// <summary>Low priority.</summary>
    Low = 0,

    /// <summary>Normal priority.</summary>
    Normal = 1,

    /// <summary>High priority.</summary>
    High = 2
}
