// PHASE 7: Scheduler.Infrastructure/Services/ConsoleEmailService.cs
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>Simulates email delivery by logging to console.</summary>
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    /// <summary>Initializes a new instance of the <see cref="ConsoleEmailService"/> class.</summary>
    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task SendAsync(EmailDeliveryDto request, CancellationToken cancellationToken)
    {
        var hasAttachment = request.Attachment is not null;
        _logger.LogInformation(
            "Email send attempted. To={To}, Subject={Subject}, HasAttachment={HasAttachment}",
            request.To,
            request.Subject,
            hasAttachment);
        return Task.CompletedTask;
    }
}
