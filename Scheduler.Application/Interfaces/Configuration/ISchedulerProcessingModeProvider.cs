// PHASE E: Scheduler.Application/Interfaces/Configuration/ISchedulerProcessingModeProvider.cs
namespace Scheduler.Application.Interfaces.Configuration;

/// <summary>Provides scheduler processing mode configuration.</summary>
public interface ISchedulerProcessingModeProvider
{
    /// <summary>When true, jobs complete immediately with hardcoded results.</summary>
    bool CompleteImmediately { get; }
}
