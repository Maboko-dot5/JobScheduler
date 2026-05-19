// PHASE D: Scheduler.Tests.Unit/Contracts/JobContractTests.cs
using System;
using System.Text.Json;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Unit.Contracts;

/// <summary>Contract tests for job DTO serialization.</summary>
public class JobContractTests
{
    /// <summary>Ensures job request includes required contract fields.</summary>
    [Fact]
    public void JobSubmissionRequest_SerializesWithRequiredFields()
    {
        var request = new JobSubmissionRequestDto("test", TaskType.Statistics, "tester")
        {
            JobId = Guid.NewGuid(),
            PlantId = "plant-001",
            RequestedTimeRange = new TimeRangeDto(DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow)
        };
        request.Variables.Add("temperature");
        request.Tasks.Add(new JobTaskDto(TaskType.Statistics)
        {
            StatisticsWindow = StatisticsWindow.Shift
        });

        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.Contains("\"plantId\"", json);
        Assert.Contains("\"jobId\"", json);
        Assert.Contains("\"variables\"", json);
        Assert.Contains("\"requestedTimeRange\"", json);
        Assert.Contains("\"statisticsWindow\"", json);
    }

    /// <summary>Ensures job submission response includes statusUrl.</summary>
    [Fact]
    public void JobSubmissionResponse_SerializesWithStatusUrl()
    {
        var response = new JobSubmissionResponseDto(Guid.NewGuid(), JobStatus.Queued)
        {
            StatusUrl = "/api/jobs/123"
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.Contains("\"statusUrl\"", json);
    }

    /// <summary>Ensures job status response includes result fields.</summary>
    [Fact]
    public void JobStatusResponse_SerializesWithResult()
    {
        var response = new JobStatusResponseDto(Guid.NewGuid(), JobStatus.Completed)
        {
            Result = new JobResultDto(Guid.NewGuid(), JobStatus.Completed)
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.Contains("\"result\"", json);
    }
}
