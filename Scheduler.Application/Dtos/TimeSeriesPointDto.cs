// PHASE 2: Scheduler.Application/Dtos/TimeSeriesPointDto.cs
using System;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a time series data point.</summary>
public class TimeSeriesPointDto
{
    /// <summary>Timestamp of the point.</summary>
    public DateTimeOffset TimestampUtc { get; set; }

    /// <summary>Variable name for the point.</summary>
    public string Variable { get; set; }

    /// <summary>Measured value.</summary>
    public double Value { get; set; }

    /// <summary>Optional quality indicator.</summary>
    public string? Quality { get; set; }

    /// <summary>Initializes a new instance of the <see cref="TimeSeriesPointDto"/> class.</summary>
    public TimeSeriesPointDto()
    {
        TimestampUtc = DateTimeOffset.MinValue;
        Variable = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="TimeSeriesPointDto"/> class with required fields.</summary>
    public TimeSeriesPointDto(DateTimeOffset timestampUtc, double value)
    {
        TimestampUtc = timestampUtc;
        Value = value;
        Variable = string.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="TimeSeriesPointDto"/> class with required fields.</summary>
    public TimeSeriesPointDto(DateTimeOffset timestampUtc, string variable, double value)
    {
        TimestampUtc = timestampUtc;
        Variable = variable;
        Value = value;
    }
}
