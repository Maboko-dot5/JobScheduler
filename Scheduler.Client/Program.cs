// PHASE 10: Scheduler.Client/Program.cs
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Enums;

namespace Scheduler.Client;

/// <summary>Console client for submitting and monitoring jobs.</summary>
public static class Program
{
    /// <summary>Application entry point.</summary>
    public static async Task Main(string[] args)
    {
        var baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : "http://localhost:5000";
        using var httpClient = new HttpClient();
        var cancellationToken = CancellationToken.None;
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        Console.WriteLine("Job Scheduler Client");
        Console.WriteLine($"API: {baseUrl}");

        var request = new JobSubmissionRequestDto
        {
            JobId = Guid.NewGuid(),
            Name = "Daily Uptime Report",
            PrimaryTaskType = TaskType.Statistics,
            RequestedBy = "console-client",
            PlantId = "PlantA",
            RequestedTimeRange = new TimeRangeDto(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow)
        };

        request.Variables.Add("temperature");
        request.Variables.Add("pressure");
        request.Tasks.Add(new JobTaskDto(TaskType.Statistics)
        {
            StatisticsWindow = StatisticsWindow.Daily
        });
        request.Tasks.Add(new JobTaskDto(TaskType.PdfReport));

        var payload = JsonSerializer.Serialize(request, jsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");

        Console.WriteLine("Submitting job...");
        var response = await httpClient.PostAsync($"{baseUrl}/api/jobs", content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Submission failed: {(int)response.StatusCode} {response.ReasonPhrase}");
            Console.WriteLine(responseBody);
            return;
        }

        var submission = JsonSerializer.Deserialize<JobSubmissionResponseDto>(responseBody, jsonOptions);
        if (submission is null)
        {
            Console.WriteLine("Submission response could not be parsed.");
            Console.WriteLine(responseBody);
            return;
        }

        Console.WriteLine($"JobId: {submission.JobId}");
        Console.WriteLine($"Initial status: {submission.Status}");

        var statusUrl = BuildStatusUrl(baseUrl, submission);
        Console.WriteLine($"Polling: {statusUrl}");

        var maxAttempts = 40;
        var delay = TimeSpan.FromMilliseconds(250);
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var statusResponse = await httpClient.GetAsync(statusUrl, cancellationToken);
            var statusBody = await statusResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!statusResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Poll {attempt}/{maxAttempts}: {(int)statusResponse.StatusCode} {statusResponse.ReasonPhrase}");
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            var status = JsonSerializer.Deserialize<JobStatusResponseDto>(statusBody, jsonOptions);
            if (status is null)
            {
                Console.WriteLine($"Poll {attempt}/{maxAttempts}: invalid status payload");
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            Console.WriteLine($"Poll {attempt}/{maxAttempts}: {status.Status} (updated {status.UpdatedAtUtc:u})");

            if (status.Status == JobStatus.Completed || status.Status == JobStatus.Failed)
            {
                Console.WriteLine("Job finished.");
                Console.WriteLine($"Summary: {status.Summary}");
                if (status.Result is not null)
                {
                    Console.WriteLine($"Result status: {status.Result.Status}");
                    Console.WriteLine($"Errors: {status.Result.Errors.Count}");
                    if (!string.IsNullOrWhiteSpace(status.Result.PayloadJson))
                    {
                        Console.WriteLine("Payload:");
                        Console.WriteLine(status.Result.PayloadJson);
                    }
                    else
                    {
                        Console.WriteLine("Payload: <empty>");
                    }
                }

                return;
            }

            await Task.Delay(delay, cancellationToken);
        }

        Console.WriteLine("Polling timed out before completion.");
    }

    private static string BuildStatusUrl(string baseUrl, JobSubmissionResponseDto submission)
    {
        var statusUrl = submission.StatusUrl;
        if (string.IsNullOrWhiteSpace(statusUrl))
        {
            return $"{baseUrl}/api/jobs/{submission.JobId}";
        }

        if (statusUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            return statusUrl;
        }

        if (!statusUrl.StartsWith("/", StringComparison.Ordinal))
        {
            statusUrl = "/" + statusUrl;
        }

        return baseUrl + statusUrl;
    }
}
