// PHASE 2: Scheduler.Application/Interfaces/Services/IStatisticsService.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines statistics calculation operations.</summary>
public interface IStatisticsService
{
    /// <summary>Calculates statistics for a request.</summary>
    Task<StatisticsResultDto> CalculateAsync(StatisticsRequestDto request, CancellationToken cancellationToken);
}
