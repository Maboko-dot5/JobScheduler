// PHASE 7: Scheduler.Infrastructure/Services/ConsoleEmailService.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Simulates email delivery by logging to console.</summary>
public class ConsoleEmailService : IEmailService
{
    /// <inheritdoc />
    public Task SendAsync(EmailDeliveryDto request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
