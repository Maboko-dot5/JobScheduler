// PHASE 2: Scheduler.Application/Interfaces/Services/IEmailService.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Interfaces.Services;

/// <summary>Defines email delivery operations.</summary>
public interface IEmailService
{
    /// <summary>Sends an email message.</summary>
    Task SendAsync(EmailDeliveryDto request, CancellationToken cancellationToken);
}
