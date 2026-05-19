// PHASE 2: Scheduler.Application/Dtos/StatisticsResultDto.cs
using System.Collections.Generic;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a statistics calculation result.</summary>
public class StatisticsResultDto
{
    /// <summary>Series identifier.</summary>
    public string SeriesId { get; set; }

    /// <summary>Computed metrics as key/value pairs.</summary>
    public Dictionary<string, double> Metrics { get; set; }

    /// <summary>Initializes a new instance of the <see cref="StatisticsResultDto"/> class.</summary>
    public StatisticsResultDto()
    {
        SeriesId = string.Empty;
        Metrics = new Dictionary<string, double>();
    }

    /// <summary>Initializes a new instance of the <see cref="StatisticsResultDto"/> class with required fields.</summary>
    public StatisticsResultDto(string seriesId)
    {
        SeriesId = seriesId;
        Metrics = new Dictionary<string, double>();
    }
}
