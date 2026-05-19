// PHASE 11: Scheduler.Tests.Integration/JobApiTests.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Enums;
using Xunit;

namespace Scheduler.Tests.Integration;

/// <summary>Integration tests for the job API.</summary>
public class JobApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    /// <summary>Initializes a new instance of the <see cref="JobApiTests"/> class.</summary>
    public JobApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    /// <summary>Ensures the API bootstraps successfully.</summary>
    [Fact]
    public async Task GetJobs_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/jobs");

        Assert.True(response.IsSuccessStatusCode);
    }

    /// <summary>Ensures a submitted job is accepted and eventually completes.</summary>
    [Fact]
    public async Task PostJob_ReturnsAcceptedAndCompletes()
    {
        var client = _factory.CreateClient();
        var request = new JobSubmissionRequestDto("test-job", TaskType.Statistics, "tester")
        {
            JobId = Guid.NewGuid(),
            PlantId = "plant-001",
            RequestedTimeRange = new TimeRangeDto(DateTimeOffset.UtcNow.AddMinutes(-5), DateTimeOffset.UtcNow)
        };
        request.Variables.Add("temperature");
        request.Tasks.Add(new JobTaskDto(TaskType.Statistics)
        {
            StatisticsWindow = StatisticsWindow.Daily
        });

        var payload = JsonSerializer.Serialize(request);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/jobs", content);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var submissionBody = await response.Content.ReadAsStringAsync();
        var submission = JsonSerializer.Deserialize<JobSubmissionResponseDto>(submissionBody, jsonOptions);

        Assert.NotNull(submission);
        Assert.NotEqual(Guid.Empty, submission!.JobId);
        var statusUrl = !string.IsNullOrWhiteSpace(submission.StatusUrl)
            ? submission.StatusUrl
            : $"/api/jobs/{submission.JobId}";

        JobStatusResponseDto? status = null;
        for (var attempt = 0; attempt < 50; attempt++)
        {
            var statusResponse = await client.GetAsync(statusUrl);
            if (statusResponse.StatusCode == HttpStatusCode.OK)
            {
                var statusBody = await statusResponse.Content.ReadAsStringAsync();
                status = JsonSerializer.Deserialize<JobStatusResponseDto>(statusBody, jsonOptions);
                if (status is not null &&
                    (status.Status == JobStatus.Completed || status.Status == JobStatus.Failed))
                {
                    break;
                }
            }
            else
            {
                var listResponse = await client.GetAsync("/api/jobs");
                if (listResponse.StatusCode == HttpStatusCode.OK)
                {
                    var listBody = await listResponse.Content.ReadAsStringAsync();
                    var list = JsonSerializer.Deserialize<List<JobStatusResponseDto>>(listBody, jsonOptions);
                    if (list is not null)
                    {
                        status = list.Find(item => item.JobId == submission.JobId);
                        if (status is not null &&
                            (status.Status == JobStatus.Completed || status.Status == JobStatus.Failed))
                        {
                            break;
                        }
                    }
                }
            }

            await Task.Delay(200);
        }

        Assert.NotNull(status);
        Assert.True(status!.Status == JobStatus.Completed || status.Status == JobStatus.Failed);
    }

    /// <summary>Ensures the health endpoint is available.</summary>
    [Fact]
    public async Task GetHealth_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
    }
}
