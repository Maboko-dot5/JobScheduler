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

/// <summary>Interactive console client for submitting and monitoring jobs.</summary>
public static class Program
{
    /// <summary>Application entry point.</summary>
    public static async Task Main(string[] args)
    {
        var baseUrl = args.Length > 0 ? args[0].TrimEnd('/') : "http://localhost:5000";
        using var httpClient = new HttpClient();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        Console.WriteLine("=== Job Scheduler Client ===");
        Console.WriteLine($"API: {baseUrl}\n");

        while (true)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("1. Submit Statistics Job");
            Console.WriteLine("2. Submit Physical Filter Job");
            Console.WriteLine("3. Submit Anomaly Filter Job");
            Console.WriteLine("4. Submit PDF Report Job");
            Console.WriteLine("5. Exit");
            Console.Write("Choose option (1-5): ");

            var choice = Console.ReadLine();
            if (choice == "5")
            {
                Console.WriteLine("Exiting...");
                break;
            }

            TaskType? selectedTaskType = choice switch
            {
                "1" => TaskType.Statistics,
                "2" => TaskType.PhysicalFilter,
                "3" => TaskType.AnomalyFilter,
                "4" => TaskType.PdfReport,
                _ => null
            };

            if (selectedTaskType is null)
            {
                Console.WriteLine("Invalid choice. Try again.");
                continue;
            }

            var request = BuildJobRequest(selectedTaskType.Value);
            await SubmitAndPollJob(httpClient, baseUrl, request, jsonOptions);
        }
    }

    private static JobSubmissionRequestDto BuildJobRequest(TaskType taskType)
    {
        Console.Write("\nEnter job name (default: 'Test Job'): ");
        var jobName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(jobName))
            jobName = "Test Job";

        Console.Write("Enter plant ID (default: 'PlantA'): ");
        var plantId = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(plantId))
            plantId = "PlantA";

        Console.Write("Enter variables (comma-separated, default: 'temperature,pressure'): ");
        var varsInput = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(varsInput))
            varsInput = "temperature,pressure";
        var variables = varsInput.Split(',', StringSplitOptions.RemoveEmptyEntries);

        Console.Write("Enter days back (default: 1): ");
        if (!int.TryParse(Console.ReadLine(), out var daysBack))
            daysBack = 1;

        var startTime = DateTimeOffset.UtcNow.AddDays(-daysBack);
        var endTime = DateTimeOffset.UtcNow;

        var request = new JobSubmissionRequestDto
        {
            JobId = Guid.NewGuid(),
            Name = jobName,
            PrimaryTaskType = taskType,
            RequestedBy = "console-client",
            PlantId = plantId,
            RequestedTimeRange = new TimeRangeDto(startTime, endTime)
        };

        foreach (var variable in variables)
        {
            request.Variables.Add(variable.Trim());
        }

        var taskDto = new JobTaskDto(taskType);
        if (taskType == TaskType.Statistics)
        {
            Console.Write("Enter statistics window (0=Shift, 1=Daily, 2=Weekly, default: 1): ");
            if (int.TryParse(Console.ReadLine(), out var windowInt) && windowInt >= 0 && windowInt <= 2)
            {
                taskDto.StatisticsWindow = (StatisticsWindow)windowInt;
            }
            else
            {
                taskDto.StatisticsWindow = StatisticsWindow.Daily;
            }
        }

        request.Tasks.Add(taskDto);
        return request;
    }

    private static async Task SubmitAndPollJob(
        HttpClient httpClient,
        string baseUrl,
        JobSubmissionRequestDto request,
        JsonSerializerOptions jsonOptions)
    {
        var cancellationToken = CancellationToken.None;
        var payload = JsonSerializer.Serialize(request, jsonOptions);
        using var content = new StringContent(payload, Encoding.UTF8, "application/json");

        Console.WriteLine("\nSubmitting job...");
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
            return;
        }

        Console.WriteLine($"✓ JobId: {submission.JobId}");
        Console.WriteLine($"✓ Initial status: {submission.Status}");

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
                Console.Write(".");
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            var status = JsonSerializer.Deserialize<JobStatusResponseDto>(statusBody, jsonOptions);
            if (status is null)
            {
                Console.Write(".");
                await Task.Delay(delay, cancellationToken);
                continue;
            }

            Console.Write(".");

            if (status.Status == JobStatus.Completed || status.Status == JobStatus.Failed)
            {
                Console.WriteLine("\n\n--- Job Result ---");
                Console.WriteLine($"Status: {status.Status}");
                Console.WriteLine($"Summary: {status.Summary}");
                if (status.Result is not null)
                {
                    Console.WriteLine($"Errors: {status.Result.Errors.Count}");
                    if (!string.IsNullOrWhiteSpace(status.Result.PayloadJson))
                    {
                        Console.WriteLine("\nTask Results:");
                        try
                        {
                            var formattedJson = JsonSerializer.Serialize(
                                JsonSerializer.Deserialize<object>(status.Result.PayloadJson, jsonOptions),
                                new JsonSerializerOptions { WriteIndented = true });
                            Console.WriteLine(formattedJson);
                        }
                        catch
                        {
                            Console.WriteLine(status.Result.PayloadJson);
                        }
                    }
                }

                return;
            }

            await Task.Delay(delay, cancellationToken);
        }

        Console.WriteLine("\nPolling timed out before completion.");
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
