// PHASE 8: Scheduler.Infrastructure/DependencyInjection.cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scheduler.Application.Background;
using Scheduler.Application.Handlers;
using Scheduler.Application.Interfaces.Configuration;
using Scheduler.Application.Interfaces.Handlers;
using Scheduler.Application.Interfaces.Queue;
using Scheduler.Application.Interfaces.Repositories;
using Scheduler.Application.Interfaces.Services;
using Scheduler.Application.Orchestration;
using Scheduler.Application.Queue;
using Scheduler.Application.Validation;
using Scheduler.Infrastructure.Configuration;
using Scheduler.Infrastructure.Repositories;
using Scheduler.Infrastructure.Services;

namespace Scheduler.Infrastructure;

/// <summary>Registers infrastructure and application services.</summary>
public static class DependencyInjection
{
    /// <summary>Adds scheduler services to the service collection.</summary>
    public static IServiceCollection AddSchedulerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISchedulerProcessingModeProvider, SchedulerProcessingModeProvider>();

        services.AddSingleton<IJobQueue, InMemoryJobQueue>();
        services.AddSingleton<IJobRepository, InMemoryJobRepository>();
        services.AddSingleton<IJobStatusRepository, InMemoryJobStatusRepository>();
        services.AddSingleton<ITimeSeriesRepository, MockTimeSeriesRepository>();

        services.AddSingleton<IStatisticsService, FakeStatisticsService>();
        services.AddSingleton<IAnomalyFilterService, FakeAnomalyFilterService>();
        services.AddSingleton<IPhysicalFilterService, FakePhysicalFilterService>();
        services.AddSingleton<IReportGenerator, FakeReportGenerator>();
        services.AddSingleton<IReportStore, InMemoryReportStore>();
        services.AddSingleton<IEmailOutbox, InMemoryEmailOutbox>();
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddSingleton<IFilteringRuleProvider, StaticFilteringRuleProvider>();

        services.AddSingleton<ITaskHandler, PhysicalFilterHandler>();
        services.AddSingleton<ITaskHandler, AnomalyFilterHandler>();
        services.AddSingleton<ITaskHandler, StatisticsHandler>();
        services.AddSingleton<ITaskHandler, PdfReportHandler>();
        services.AddSingleton<ITaskHandlerRegistry, TaskHandlerRegistry>();

        services.AddSingleton<IJobOrchestrator, JobOrchestrator>();
        services.AddSingleton<JobRequestValidator>();

        services.AddHostedService<BackgroundJobWorker>();
        services.AddHostedService<EmailOutboxWorker>();

        return services;
    }
}
