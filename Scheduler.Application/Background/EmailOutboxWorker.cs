// PHASE 12: Scheduler.Application/Background/EmailOutboxWorker.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Application.Background;

/// <summary>Background worker that delivers queued email outbox messages.</summary>
public class EmailOutboxWorker : BackgroundService
{
    private readonly IEmailOutbox _emailOutbox;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailOutboxWorker> _logger;

    /// <summary>Initializes a new instance of the <see cref="EmailOutboxWorker"/> class.</summary>
    public EmailOutboxWorker(IEmailOutbox emailOutbox, IEmailService emailService, ILogger<EmailOutboxWorker> logger)
    {
        _emailOutbox = emailOutbox;
        _emailService = emailService;
        _logger = logger;
    }

    /// <summary>Executes the background delivery loop.</summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            EmailDeliveryDto? message = null;
            try
            {
                message = await _emailOutbox.DequeueAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (message is null)
            {
                continue;
            }

            try
            {
                await _emailService.SendAsync(message, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver outbox email.");
            }
        }
    }
}
