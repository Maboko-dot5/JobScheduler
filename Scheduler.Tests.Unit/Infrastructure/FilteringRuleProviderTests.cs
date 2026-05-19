// PHASE E: Scheduler.Tests.Unit/Infrastructure/FilteringRuleProviderTests.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Infrastructure.Services;
using Xunit;

namespace Scheduler.Tests.Unit.Infrastructure;

/// <summary>Unit tests for <see cref="StaticFilteringRuleProvider"/>.</summary>
public class FilteringRuleProviderTests
{
    /// <summary>Ensures temperature rules are applied.</summary>
    [Fact]
    public async Task GetPhysicalRuleAsync_ReturnsTemperatureRule()
    {
        var provider = new StaticFilteringRuleProvider();

        var rule = await provider.GetPhysicalRuleAsync("plant-001", "temperature", CancellationToken.None);

        Assert.Equal(0, rule.MinValue);
        Assert.Equal(100, rule.MaxValue);
    }
}
