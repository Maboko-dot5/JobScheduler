// PHASE E: Scheduler.Tests.Unit/Infrastructure/ConsoleEmailServiceTests.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Infrastructure.Services;
using Xunit;

namespace Scheduler.Tests.Unit.Infrastructure;

/// <summary>Unit tests for <see cref="ConsoleEmailService"/>.</summary>
public class ConsoleEmailServiceTests
{
    /// <summary>Ensures email send completes without error.</summary>
    [Fact]
    public async Task SendAsync_Completes()
    {
        var service = new ConsoleEmailService();
        var request = new EmailDeliveryDto("user@example.com", "subject", "body");

        await service.SendAsync(request, CancellationToken.None);

        Assert.True(true);
    }
}
