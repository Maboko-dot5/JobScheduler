// PHASE 2: Scheduler.Application/Dtos/AnomalyFilterRequestDto.cs
using System.Collections.Generic;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Dtos;

/// <summary>Represents an anomaly filter request.</summary>
public class AnomalyFilterRequestDto
{
    /// <summary>Series identifier.</summary>
    public string SeriesId { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variable name.</summary>
    public string Variable { get; set; }

    /// <summary>Anomaly filtering rule.</summary>
    public AnomalyFilterRule? Rule { get; set; }

    /// <summary>Input data points.</summary>
    public List<TimeSeriesPointDto> Points { get; set; }

    /// <summary>Initializes a new instance of the <see cref="AnomalyFilterRequestDto"/> class.</summary>
    public AnomalyFilterRequestDto()
    {
        SeriesId = string.Empty;
        PlantId = string.Empty;
        Variable = string.Empty;
        Points = new List<TimeSeriesPointDto>();
    }

    /// <summary>Initializes a new instance of the <see cref="AnomalyFilterRequestDto"/> class with required fields.</summary>
    public AnomalyFilterRequestDto(string seriesId)
    {
        SeriesId = seriesId;
        PlantId = string.Empty;
        Variable = string.Empty;
        Points = new List<TimeSeriesPointDto>();
    }
}
