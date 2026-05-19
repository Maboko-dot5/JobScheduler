// PHASE 7: Scheduler.Infrastructure/Services/StaticFilteringRuleProvider.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Infrastructure.Services;

/// <summary>Provides static filtering rules.</summary>
public class StaticFilteringRuleProvider : IFilteringRuleProvider
{
    /// <inheritdoc />
    public Task<PhysicalFilterRule> GetPhysicalRuleAsync(string plantId, string variable, CancellationToken cancellationToken)
    {
        var rule = variable.ToLowerInvariant() switch
        {
            "temperature" => new PhysicalFilterRule(0, 100),
            "pressure" => new PhysicalFilterRule(0, 300),
            "vibration" => new PhysicalFilterRule(0, 50),
            _ => new PhysicalFilterRule(-1000, 1000)
        };

        return Task.FromResult(rule);
    }

    /// <inheritdoc />
    public Task<AnomalyFilterRule> GetAnomalyRuleAsync(string plantId, string variable, CancellationToken cancellationToken)
    {
        return Task.FromResult(new AnomalyFilterRule(3.0, 10));
    }
}
