// PHASE 2: Scheduler.Application/Interfaces/Handlers/ITaskHandlerRegistry.cs
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Interfaces.Handlers;

/// <summary>Provides task handlers by task type.</summary>
public interface ITaskHandlerRegistry
{
    /// <summary>Gets a handler for the specified task type.</summary>
    ITaskHandler? GetHandler(TaskType taskType);
}
