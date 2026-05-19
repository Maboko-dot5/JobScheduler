// PHASE 1: Scheduler.Domain/Enums/StatisticsWindow.cs
namespace Scheduler.Domain.Enums;

/// <summary>Represents the statistics window types.</summary>
public enum StatisticsWindow
{
    /// <summary>Shift-based statistics.</summary>
    Shift = 0,

    /// <summary>Daily statistics.</summary>
    Daily = 1,

    /// <summary>Weekly statistics.</summary>
    Weekly = 2
}
