// PHASE 6: Scheduler.Application/Validation/JobRequestValidator.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;

namespace Scheduler.Application.Validation;

/// <summary>Validates job submission requests.</summary>
public class JobRequestValidator
{
    /// <summary>Validates a job submission request.</summary>
    public Task<ValidationResultDto> ValidateAsync(JobSubmissionRequestDto request, CancellationToken cancellationToken)
    {
        var result = new ValidationResultDto(true);

        if (request.JobId == Guid.Empty)
        {
            result.IsValid = false;
            result.Errors.Add("jobId is required.");
        }

        return Task.FromResult(result);
    }
}
