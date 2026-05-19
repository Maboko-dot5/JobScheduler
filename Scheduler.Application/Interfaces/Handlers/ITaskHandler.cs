// PHASE 2: Scheduler.Application/Interfaces/Handlers/ITaskHandler.cs
using System.Threading;
using System.Threading.Tasks;
using Scheduler.Application.Dtos;
using Scheduler.Domain.Entities;

namespace Scheduler.Application.Interfaces.Handlers;

/// <summary>Defines a handler for a specific task type.</summary>
public interface ITaskHandler
{
    /// <summary>Executes a job task.</summary>
    Task<TaskExecutionResultDto> ExecuteAsync(JobContextDto context, CancellationToken cancellationToken);
}
