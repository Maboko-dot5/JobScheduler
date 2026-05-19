// PHASE 2: Scheduler.Application/Dtos/StatisticsRequestDto.cs
using System.Collections.Generic;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a statistics calculation request.</summary>
public class StatisticsRequestDto
{
    /// <summary>Series identifier.</summary>
    public string SeriesId { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variables to include.</summary>
    public List<string> Variables { get; set; }

    /// <summary>Statistics window.</summary>
    public StatisticsWindow Window { get; set; }

    /// <summary>Time range for the calculation.</summary>
    public TimeRangeDto? TimeRange { get; set; }

    /// <summary>Input data points.</summary>
    public List<TimeSeriesPointDto> Points { get; set; }

    /// <summary>Initializes a new instance of the <see cref="StatisticsRequestDto"/> class.</summary>
    public StatisticsRequestDto()
    {
        SeriesId = string.Empty;
        PlantId = string.Empty;
        Variables = new List<string>();
        Points = new List<TimeSeriesPointDto>();
    }

    /// <summary>Initializes a new instance of the <see cref="StatisticsRequestDto"/> class with required fields.</summary>
    public StatisticsRequestDto(string seriesId, StatisticsWindow window)
    {
        SeriesId = seriesId;
        Window = window;
        PlantId = string.Empty;
        Variables = new List<string>();
        Points = new List<TimeSeriesPointDto>();
    }
}
