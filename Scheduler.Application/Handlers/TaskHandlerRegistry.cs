// PHASE 5: Scheduler.Application/Handlers/TaskHandlerRegistry.cs
using System.Collections.Generic;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Domain.Enums;

namespace Scheduler.Application.Handlers;

/// <summary>In-memory registry for task handlers.</summary>
public class TaskHandlerRegistry : ITaskHandlerRegistry
{
    private readonly Dictionary<TaskType, ITaskHandler> _handlers;

    /// <summary>Initializes a new instance of the <see cref="TaskHandlerRegistry"/> class.</summary>
    public TaskHandlerRegistry(IEnumerable<ITaskHandler> handlers)
    {
        _handlers = new Dictionary<TaskType, ITaskHandler>();

        foreach (var handler in handlers)
        {
            if (handler is PhysicalFilterHandler)
            {
                _handlers[TaskType.PhysicalFilter] = handler;
            }
            else if (handler is AnomalyFilterHandler)
            {
                _handlers[TaskType.AnomalyFilter] = handler;
            }
            else if (handler is StatisticsHandler)
            {
                _handlers[TaskType.Statistics] = handler;
            }
            else if (handler is PdfReportHandler)
            {
                _handlers[TaskType.PdfReport] = handler;
            }
        }
    }

    /// <inheritdoc />
    public ITaskHandler? GetHandler(TaskType taskType)
    {
        return _handlers.TryGetValue(taskType, out var handler) ? handler : null;
    }
}
