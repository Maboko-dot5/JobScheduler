using Scheduler.Domain.Enums;

namespace Scheduler.Application.Dtos;

/// <summary>Represents statistics calculated for one report window.</summary>
public class ReportStatisticsSummaryDto
{
    /// <summary>Statistics window represented by this summary.</summary>
    public StatisticsWindow Window { get; set; }

    /// <summary>Calculated statistics for the window.</summary>
    public StatisticsResultDto Statistics { get; set; }

    /// <summary>Initializes a new instance of the <see cref="ReportStatisticsSummaryDto"/> class.</summary>
    public ReportStatisticsSummaryDto()
    {
        Statistics = new StatisticsResultDto();
    }

    /// <summary>Initializes a new instance of the <see cref="ReportStatisticsSummaryDto"/> class with required fields.</summary>
    public ReportStatisticsSummaryDto(StatisticsWindow window, StatisticsResultDto statistics)
    {
        Window = window;
        Statistics = statistics;
    }
}
