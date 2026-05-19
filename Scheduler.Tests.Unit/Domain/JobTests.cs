// PHASE 11: Scheduler.Tests.Unit/Domain/JobTests.cs
using Scheduler.Domain.Entities;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;
using Xunit;

namespace Scheduler.Tests.Unit.Domain;

/// <summary>Unit tests for <see cref="Job"/>.</summary>
public class JobTests
{
    /// <summary>Ensures a job can be created with required fields.</summary>
    [Fact]
    public void Constructor_SetsRequiredFields()
    {
        var jobId = new JobId(System.Guid.NewGuid());
        var job = new Job(jobId, "test", TaskType.PhysicalFilter, "user");

        Assert.Equal(jobId, job.Id);
        Assert.Equal("test", job.Name);
    }
}
