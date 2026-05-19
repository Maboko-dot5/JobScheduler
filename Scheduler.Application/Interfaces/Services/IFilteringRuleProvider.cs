// PHASE 2: Scheduler.Application/Interfaces/Services/IFilteringRuleProvider.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines access to filtering rules.</summary>
public interface IFilteringRuleProvider
{
    /// <summary>Gets physical filtering rules for a plant/variable.</summary>
    Task<PhysicalFilterRule> GetPhysicalRuleAsync(string plantId, string variable, CancellationToken cancellationToken);

    /// <summary>Gets anomaly filtering rules for a plant/variable.</summary>
    Task<AnomalyFilterRule> GetAnomalyRuleAsync(string plantId, string variable, CancellationToken cancellationToken);
}
