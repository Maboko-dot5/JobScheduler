// PHASE 4: Scheduler.Application/Background/BackgroundJobWorker.cs
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Application.Interfaces.Queue;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Background;

/// <summary>Background worker that processes jobs from the queue.</summary>
public class BackgroundJobWorker : BackgroundService
{
    private readonly IJobQueue _jobQueue;
    private readonly ITaskHandlerRegistry _taskHandlerRegistry;
    private readonly IJobStatusRepository _jobStatusRepository;
    private readonly IJobRepository _jobRepository;
    private readonly ILogger<BackgroundJobWorker> _logger;

    /// <summary>Initializes a new instance of the <see cref="BackgroundJobWorker"/> class.</summary>
    public BackgroundJobWorker(
        IJobQueue jobQueue,
        ITaskHandlerRegistry taskHandlerRegistry,
        IJobStatusRepository jobStatusRepository,
        IJobRepository jobRepository,
        ILogger<BackgroundJobWorker> logger)
    {
        _jobQueue = jobQueue;
        _taskHandlerRegistry = taskHandlerRegistry;
        _jobStatusRepository = jobStatusRepository;
        _jobRepository = jobRepository;
        _logger = logger;
    }

    /// <summary>Executes the background processing loop.</summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var job = await _jobQueue.DequeueAsync(stoppingToken);
            if (job is null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250), stoppingToken);
                continue;
            }

            await ProcessJobAsync(job, stoppingToken);
        }
    }

    private async Task ProcessJobAsync(Job job, CancellationToken cancellationToken)
    {
        var taskSummaries = new List<TaskSummaryDto>();
        job.Status = JobStatus.Running;
        job.StartedAtUtc = DateTimeOffset.UtcNow;
        job.UpdatedAtUtc = DateTimeOffset.UtcNow;
        await _jobStatusRepository.UpdateStatusAsync(job.Id, JobStatus.Running, cancellationToken);
        await _jobRepository.UpdateAsync(job, cancellationToken);

        var result = new JobResult(job.Id, JobStatus.Completed)
        {
            Summary = "Completed",
            CompletedAtUtc = DateTimeOffset.UtcNow,
            Errors = new List<JobError>()
        };

        var allSucceeded = true;

        foreach (var task in job.Tasks)
        {
            var handler = _taskHandlerRegistry.GetHandler(task.TaskType);
            if (handler is null)
            {
                allSucceeded = false;
                result.Errors.Add(new JobError("handler_not_found", "No handler registered for task type."));
                continue;
            }

            var timeRange = task.TimeRange ?? job.RequestedTimeRange ?? new TimeRange(DateTimeOffset.MinValue, DateTimeOffset.MinValue);
            var context = new JobContextDto(job.Id.Value, task.TaskType, job.PlantId, new TimeRangeDto(timeRange.StartUtc, timeRange.EndUtc))
            {
                StatisticsWindow = task.StatisticsWindow,
                ParametersJson = task.ParametersJson
            };
            context.Variables.AddRange(job.Variables);

            var taskResult = await handler.ExecuteAsync(context, cancellationToken);
            taskSummaries.Add(new TaskSummaryDto
            {
                TaskType = task.TaskType,
                Succeeded = taskResult.Succeeded,
                Summary = taskResult.Summary,
                OutputLocation = taskResult.OutputLocation
            });
            if (!taskResult.Succeeded)
            {
                allSucceeded = false;
                if (taskResult.Errors.Count > 0)
                {
                    result.Errors.AddRange(taskResult.Errors);
                }
                else
                {
                    result.Errors.Add(new JobError("task_failed", "Task execution failed."));
                }
            }
        }

        job.Status = allSucceeded ? JobStatus.Completed : JobStatus.Failed;
        job.CompletedAtUtc = DateTimeOffset.UtcNow;
        job.UpdatedAtUtc = DateTimeOffset.UtcNow;
        result.Status = job.Status;
        result.CompletedAtUtc = job.CompletedAtUtc ?? DateTimeOffset.UtcNow;
        result.PayloadJson = JsonSerializer.Serialize(taskSummaries);
        job.Result = result;

        await _jobStatusRepository.UpdateStatusAsync(job.Id, job.Status, cancellationToken);
        await _jobRepository.UpdateAsync(job, cancellationToken);
    }

    private sealed class TaskSummaryDto
    {
        public TaskType TaskType { get; set; }

        public bool Succeeded { get; set; }

        public string? Summary { get; set; }

        public string? OutputLocation { get; set; }
    }
}
