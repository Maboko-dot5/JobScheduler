// PHASE 1: Scheduler.Domain/Enums/TaskType.cs
namespace Scheduler.Domain.Enums;

/// <summary>Represents the supported task types.</summary>
public enum TaskType
{
    /// <summary>Physical filtering task.</summary>
    PhysicalFilter = 0,

    /// <summary>Anomaly filtering task.</summary>
    AnomalyFilter = 1,

    /// <summary>Statistics calculation task.</summary>
    Statistics = 2,

    /// <summary>PDF report generation task.</summary>
    PdfReport = 3
}
