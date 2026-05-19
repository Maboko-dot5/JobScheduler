// PHASE 1: Scheduler.Domain/ValueObjects/JobId.cs
using System;

namespace Scheduler.Domain.ValueObjects;

/// <summary>Represents a strong-typed job identifier.</summary>
public class JobId
{
    /// <summary>Underlying identifier value.</summary>
    public Guid Value { get; set; }

    /// <summary>Initializes a new instance of the <see cref="JobId"/> class.</summary>
    public JobId()
    {
        Value = Guid.Empty;
    }

    /// <summary>Initializes a new instance of the <see cref="JobId"/> class with a value.</summary>
    public JobId(Guid value)
    {
        Value = value;
    }
}
