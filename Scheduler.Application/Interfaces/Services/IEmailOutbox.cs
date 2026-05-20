// PHASE 12: Scheduler.Application/Interfaces/Services/IEmailOutbox.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines an email outbox for queued deliveries.</summary>
public interface IEmailOutbox
{
    /// <summary>Queues an email for later delivery.</summary>
    Task EnqueueAsync(EmailDeliveryDto request, CancellationToken cancellationToken);

    /// <summary>Dequeues the next email to deliver.</summary>
    Task<EmailDeliveryDto?> DequeueAsync(CancellationToken cancellationToken);
}
