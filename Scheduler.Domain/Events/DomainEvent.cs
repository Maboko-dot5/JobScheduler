// PHASE 1: Scheduler.Domain/Events/DomainEvent.cs
using System;

namespace Scheduler.Domain.Events;

/// <summary>Base type for domain events.</summary>
public abstract class DomainEvent
{
    /// <summary>Unique event identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Timestamp the event occurred.</summary>
    public DateTimeOffset OccurredAtUtc { get; set; }

    /// <summary>Initializes a new instance of the <see cref="DomainEvent"/> class.</summary>
    protected DomainEvent()
    {
        Id = Guid.Empty;
        OccurredAtUtc = DateTimeOffset.MinValue;
    }
}
