// PHASE 2: Scheduler.Application/Dtos/TimeRangeDto.cs
using System;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a time range for DTOs.</summary>
public class TimeRangeDto
{
    /// <summary>Start time in UTC.</summary>
    public DateTimeOffset StartUtc { get; set; }

    /// <summary>End time in UTC.</summary>
    public DateTimeOffset EndUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="TimeRangeDto"/> class.</summary>
    public TimeRangeDto()
    {
        StartUtc = DateTimeOffset.MinValue;
        EndUtc = DateTimeOffset.MinValue;
    }

    /// <summary>Initializes a new instance of the <see cref="TimeRangeDto"/> class with required fields.</summary>
    public TimeRangeDto(DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }
}
