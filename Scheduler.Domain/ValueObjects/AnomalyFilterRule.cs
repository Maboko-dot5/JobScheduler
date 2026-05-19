// PHASE 1: Scheduler.Domain/ValueObjects/AnomalyFilterRule.cs
namespace Scheduler.Domain.ValueObjects;

/// <summary>Represents anomaly filtering parameters.</summary>
public class AnomalyFilterRule
{
    /// <summary>Z-score threshold for detecting outliers.</summary>
    public double ZScoreThreshold { get; set; }

    /// <summary>Window size for anomaly detection.</summary>
    public int WindowSize { get; set; }

    /// <summary>Initializes a new instance of the <see cref="AnomalyFilterRule"/> class.</summary>
    public AnomalyFilterRule()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="AnomalyFilterRule"/> class with required fields.</summary>
    public AnomalyFilterRule(double zScoreThreshold, int windowSize)
    {
        ZScoreThreshold = zScoreThreshold;
        WindowSize = windowSize;
    }
}
