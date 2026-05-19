// PHASE 11: Scheduler.Tests.Unit/Validation/JobRequestValidatorTests.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Validation;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Unit.Validation;

/// <summary>Unit tests for <see cref="JobRequestValidator"/>.</summary>
public class JobRequestValidatorTests
{
    /// <summary>Ensures validation returns a result.</summary>
    [Fact]
    public async Task ValidateAsync_ReturnsResult()
    {
        var validator = new JobRequestValidator();
        var request = new JobSubmissionRequestDto("test", TaskType.Statistics, "user")
        {
            JobId = System.Guid.NewGuid(),
            PlantId = "plant-001",
            RequestedTimeRange = new TimeRangeDto(DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow)
        };
        request.Variables.Add("temperature");

        var result = await validator.ValidateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
    }
}
