// PHASE 2: Scheduler.Application/Dtos/PdfReportRequestDto.cs
using System.Collections.Generic;

namespace Scheduler.Application.Dtos;

/// <summary>Represents a PDF report generation request.</summary>
public class PdfReportRequestDto
{
    /// <summary>Series identifier.</summary>
    public string SeriesId { get; set; }

    /// <summary>Plant identifier.</summary>
    public string PlantId { get; set; }

    /// <summary>Variables to include.</summary>
    public List<string> Variables { get; set; }

    /// <summary>Time range for the report.</summary>
    public TimeRangeDto? TimeRange { get; set; }

    /// <summary>Statistics summary to include.</summary>
    public StatisticsResultDto? Statistics { get; set; }

    /// <summary>Statistics summaries to include, grouped by reporting window.</summary>
    public List<ReportStatisticsSummaryDto> StatisticsSummaries { get; set; }

    /// <summary>Initializes a new instance of the <see cref="PdfReportRequestDto"/> class.</summary>
    public PdfReportRequestDto()
    {
        SeriesId = string.Empty;
        PlantId = string.Empty;
        Variables = new List<string>();
        StatisticsSummaries = new List<ReportStatisticsSummaryDto>();
    }

    /// <summary>Initializes a new instance of the <see cref="PdfReportRequestDto"/> class with required fields.</summary>
    public PdfReportRequestDto(string seriesId)
    {
        SeriesId = seriesId;
        PlantId = string.Empty;
        Variables = new List<string>();
        StatisticsSummaries = new List<ReportStatisticsSummaryDto>();
    }
}
