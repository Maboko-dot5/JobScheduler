// PHASE 12: Scheduler.Infrastructure/Services/InMemoryEmailOutbox.cs
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Services;

namespace Scheduler.Infrastructure.Services;

/// <summary>In-memory outbox for queued email delivery.</summary>
public class InMemoryEmailOutbox : IEmailOutbox
{
    private readonly ConcurrentQueue<EmailDeliveryDto> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);

    /// <inheritdoc />
    public Task EnqueueAsync(EmailDeliveryDto request, CancellationToken cancellationToken)
    {
        _queue.Enqueue(request);
        _signal.Release();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<EmailDeliveryDto?> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        return _queue.TryDequeue(out var item) ? item : null;
    }
}
