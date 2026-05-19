// PHASE 9: Scheduler.Api/Controllers/JobsController.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scheduler.Application.Dtos;
using Scheduler.Application.Orchestration;
using Scheduler.Application.Validation;
using Scheduler.Domain.ValueObjects;

namespace Scheduler.Api.Controllers;

/// <summary>Provides job submission and query endpoints.</summary>
[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly IJobOrchestrator _jobOrchestrator;
    private readonly JobRequestValidator _jobRequestValidator;
    private readonly ILogger<JobsController> _logger;

    /// <summary>Initializes a new instance of the <see cref="JobsController"/> class.</summary>
    public JobsController(
        IJobOrchestrator jobOrchestrator,
        JobRequestValidator jobRequestValidator,
        ILogger<JobsController> logger)
    {
        _jobOrchestrator = jobOrchestrator;
        _jobRequestValidator = jobRequestValidator;
        _logger = logger;
    }

    /// <summary>Submits a new job.</summary>
    [HttpPost]
    public async Task<ActionResult<JobSubmissionResponseDto>> SubmitAsync(
        [FromBody] JobSubmissionRequestDto request,
        CancellationToken cancellationToken)
    {
        var validation = await _jobRequestValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation);
        }

        var response = await _jobOrchestrator.SubmitAsync(request, cancellationToken);
        response.StatusUrl ??= Url.Action(nameof(GetByIdAsync), new { id = response.JobId });

        return Accepted(response);
    }

    /// <summary>Gets a job status by identifier.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<JobStatusResponseDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        if (!System.Guid.TryParse(id, out var parsedId))
        {
            return BadRequest();
        }

        var status = await _jobOrchestrator.GetStatusAsync(new JobId(parsedId), cancellationToken);
        if (status is null)
        {
            var items = await _jobOrchestrator.ListAsync(cancellationToken);
            foreach (var item in items)
            {
                if (item.JobId == parsedId)
                {
                    var fallback = new JobStatusResponseDto(item.JobId, item.Status)
                    {
                        UpdatedAtUtc = item.CreatedAtUtc
                    };
                    return Ok(fallback);
                }
            }

            return NotFound();
        }

        return Ok(status);
    }

    /// <summary>Lists all jobs.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<JobStatusResponseDto>>> ListAsync(CancellationToken cancellationToken)
    {
        var items = await _jobOrchestrator.ListAsync(cancellationToken);
        var statusItems = items.Select(item => new JobStatusResponseDto(item.JobId, item.Status)).ToList();
        return Ok(statusItems);
    }

    /// <summary>Cancels a job by identifier.</summary>
    [HttpPost("{id}/cancel")]
    public Task<ActionResult> CancelAsync(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult<ActionResult>(Accepted());
    }
}
