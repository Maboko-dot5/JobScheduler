// PHASE 3: Scheduler.Application/Orchestration/JobOrchestrator.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Application.Interfaces.Configuration;
using Scheduler.Application.Interfaces.Queue;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Domain.Entities;
using Scheduler.Domain.Enums;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Application.Orchestration;

/// <summary>Coordinates job submission and retrieval.</summary>
public class JobOrchestrator : IJobOrchestrator
{
    private readonly IJobQueue _jobQueue;
    private readonly IJobRepository _jobRepository;
    private readonly IJobStatusRepository _jobStatusRepository;
    private readonly ISchedulerProcessingModeProvider _processingModeProvider;

    /// <summary>Initializes a new instance of the <see cref="JobOrchestrator"/> class.</summary>
    public JobOrchestrator(
        IJobQueue jobQueue,
        IJobRepository jobRepository,
        IJobStatusRepository jobStatusRepository,
        ISchedulerProcessingModeProvider processingModeProvider)
    {
        _jobQueue = jobQueue;
        _jobRepository = jobRepository;
        _jobStatusRepository = jobStatusRepository;
        _processingModeProvider = processingModeProvider;
    }

    /// <summary>Submits a new job to the system.</summary>
    public async Task<JobSubmissionResponseDto> SubmitAsync(JobSubmissionRequestDto request, CancellationToken cancellationToken)
    {
        var completeImmediately = _processingModeProvider.CompleteImmediately;
        var jobId = new JobId(request.JobId);
        var job = new Job(jobId, request.Name, request.PrimaryTaskType, request.RequestedBy)
        {
            Description = request.Description,
            RequestedTimeRange = new TimeRange(request.RequestedTimeRange.StartUtc, request.RequestedTimeRange.EndUtc),
            CorrelationId = request.CorrelationId,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow,
            Status = JobStatus.Queued,
            PlantId = request.PlantId
        };

        job.Variables.AddRange(request.Variables);

        foreach (var taskDto in request.Tasks)
        {
            var task = new JobTask(Guid.NewGuid(), jobId, taskDto.TaskType)
            {
                TimeRange = taskDto.TimeRange is null
                    ? new TimeRange(request.RequestedTimeRange.StartUtc, request.RequestedTimeRange.EndUtc)
                    : new TimeRange(taskDto.TimeRange.StartUtc, taskDto.TimeRange.EndUtc),
                StatisticsWindow = taskDto.StatisticsWindow,
                ParametersJson = taskDto.ParametersJson,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };

            job.Tasks.Add(task);
        }

        await _jobRepository.AddAsync(job, cancellationToken);
        await _jobStatusRepository.UpdateStatusAsync(job.Id, JobStatus.Queued, cancellationToken);

        if (!completeImmediately)
        {
            await _jobQueue.EnqueueAsync(job, cancellationToken);
        }

        if (completeImmediately)
        {
            // Placeholder completion for Q2 hard-coded results.
            job.Status = JobStatus.Completed;
            job.CompletedAtUtc = DateTimeOffset.UtcNow;
            job.UpdatedAtUtc = DateTimeOffset.UtcNow;
            job.Result = new JobResult(job.Id, JobStatus.Completed)
            {
                Summary = "Completed",
                CompletedAtUtc = job.CompletedAtUtc ?? DateTimeOffset.UtcNow
            };
            await _jobStatusRepository.UpdateStatusAsync(job.Id, JobStatus.Completed, cancellationToken);
            await _jobRepository.UpdateAsync(job, cancellationToken);
            return new JobSubmissionResponseDto(jobId.Value, JobStatus.Completed);
        }

        return new JobSubmissionResponseDto(jobId.Value, JobStatus.Queued);
    }

    /// <summary>Gets the status of a job.</summary>
    public Task<JobStatusResponseDto?> GetStatusAsync(JobId jobId, CancellationToken cancellationToken)
    {
        return GetStatusInternalAsync(jobId, cancellationToken);
    }

    /// <summary>Lists all jobs.</summary>
    public Task<IReadOnlyList<JobListItemDto>> ListAsync(CancellationToken cancellationToken)
    {
        return ListInternalAsync(cancellationToken);
    }

    /// <summary>Gets job details by identifier.</summary>
    public Task<JobDetailsDto?> GetDetailsAsync(JobId jobId, CancellationToken cancellationToken)
    {
        return GetDetailsInternalAsync(jobId, cancellationToken);
    }

    private async Task<JobStatusResponseDto?> GetStatusInternalAsync(JobId jobId, CancellationToken cancellationToken)
    {
        var status = await _jobStatusRepository.GetStatusAsync(jobId, cancellationToken);
        var job = await _jobRepository.GetByIdAsync(jobId, cancellationToken);

        if (status is null && job is null)
        {
            return null;
        }

        var resolvedStatus = status ?? job!.Status;
        var response = new JobStatusResponseDto(jobId.Value, resolvedStatus)
        {
            Summary = job?.Result?.Summary,
            Result = job?.Result is null ? null : MapResult(job.Result),
            UpdatedAtUtc = job?.UpdatedAtUtc,
            CompletedAtUtc = job?.CompletedAtUtc
        };

        return response;
    }

    private async Task<IReadOnlyList<JobListItemDto>> ListInternalAsync(CancellationToken cancellationToken)
    {
        var jobs = await _jobRepository.ListAsync(cancellationToken);
        var items = new List<JobListItemDto>();

        foreach (var job in jobs)
        {
            items.Add(new JobListItemDto(job.Id.Value, job.Name, job.Status)
            {
                CreatedAtUtc = job.CreatedAtUtc
            });
        }

        return items;
    }

    private async Task<JobDetailsDto?> GetDetailsInternalAsync(JobId jobId, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(jobId, cancellationToken);
        if (job is null)
        {
            return null;
        }

        var status = new JobStatusResponseDto(job.Id.Value, job.Status)
        {
            Summary = job.Result?.Summary,
            Result = job.Result is null ? null : MapResult(job.Result),
            UpdatedAtUtc = job.UpdatedAtUtc,
            CompletedAtUtc = job.CompletedAtUtc
        };

        var details = new JobDetailsDto(status);
        return details;
    }

    private static JobResultDto MapResult(JobResult result)
    {
        return new JobResultDto(result.JobId.Value, result.Status)
        {
            Summary = result.Summary,
            PayloadJson = result.PayloadJson,
            Errors = result.Errors,
            CompletedAtUtc = result.CompletedAtUtc
        };
    }
}
