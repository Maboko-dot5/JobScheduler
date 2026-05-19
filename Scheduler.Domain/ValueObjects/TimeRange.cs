// PHASE 1: Scheduler.Domain/ValueObjects/TimeRange.cs
using System;

namespace Scheduler.Domain.ValueObjects;

/// <summary>Represents a UTC time range.</summary>
public class TimeRange
{
    /// <summary>Start time in UTC.</summary>
    public DateTimeOffset StartUtc { get; set; }

    /// <summary>End time in UTC.</summary>
    public DateTimeOffset EndUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="TimeRange"/> class.</summary>
    public TimeRange()
    {
        StartUtc = DateTimeOffset.MinValue;
        EndUtc = DateTimeOffset.MinValue;
    }

    /// <summary>Initializes a new instance of the <see cref="TimeRange"/> class with required fields.</summary>
    public TimeRange(DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }
}
