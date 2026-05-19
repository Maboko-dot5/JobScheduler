// PHASE 2: Scheduler.Application/Dtos/PhysicalFilterRequestDto.cs
using System.Collections.Generic;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a physical filter request.</summary>
public class PhysicalFilterRequestDto
{
    /// <summary>Series identifier.</summary>
    public string SeriesId { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variable name.</summary>
    public string Variable { get; set; }

    /// <summary>Filtering rule.</summary>
    public PhysicalFilterRule? Rule { get; set; }

    /// <summary>Input data points.</summary>
    public List<TimeSeriesPointDto> Points { get; set; }

    /// <summary>Initializes a new instance of the <see cref="PhysicalFilterRequestDto"/> class.</summary>
    public PhysicalFilterRequestDto()
    {
        SeriesId = string.Empty;
        PlantId = string.Empty;
        Variable = string.Empty;
        Points = new List<TimeSeriesPointDto>();
    }

    /// <summary>Initializes a new instance of the <see cref="PhysicalFilterRequestDto"/> class with required fields.</summary>
    public PhysicalFilterRequestDto(string seriesId)
    {
        SeriesId = seriesId;
        PlantId = string.Empty;
        Variable = string.Empty;
        Points = new List<TimeSeriesPointDto>();
    }
}
