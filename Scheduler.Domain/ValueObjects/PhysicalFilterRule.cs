// PHASE 1: Scheduler.Domain/ValueObjects/PhysicalFilterRule.cs
namespace Scheduler.Domain.ValueObjects;

/// <summary>Represents physical filtering limits.</summary>
public class PhysicalFilterRule
{
    /// <summary>Minimum acceptable value.</summary>
    public double MinValue { get; set; }

    /// <summary>Maximum acceptable value.</summary>
    public double MaxValue { get; set; }

    /// <summary>Initializes a new instance of the <see cref="PhysicalFilterRule"/> class.</summary>
    public PhysicalFilterRule()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="PhysicalFilterRule"/> class with required fields.</summary>
    public PhysicalFilterRule(double minValue, double maxValue)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
}
