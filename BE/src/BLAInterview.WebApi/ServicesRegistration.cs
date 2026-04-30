using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.Delete;
using BLAInterview.Application.Tasks.List;
using BLAInterview.Application.Tasks.Update;
using BLAInterview.Infrastructure.Tasks;
using FluentValidation;

namespace BLAInterview.WebApi;

/// <summary>
/// Registers services for the application.
/// </summary>
public static class ServicesRegistration
{
    /// <summary>
    /// Adds the task repository to the service collection.
    /// </summary>
    public static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ITaskRepository, TaskRepository>();
    }

    public static void AddValidators(IServiceCollection services)
    {
        services.AddScoped<IValidator<CreateTaskCommand>, CreateTaskCommandValidator>();
        services.AddScoped<IValidator<UpdateTaskCommand>, UpdateTaskCommandValidator>();
    }

    public static void AddCommandHandlers(IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateTaskCommand, int>, CreateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateTaskCommand, TaskDto>, UpdateTaskCommandHandler>();
        services.AddScoped<ICommandHandler<DeleteTaskCommand, bool>, DeleteTaskCommandHandler>();
    }

    public static void AddQueryHandlers(IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<ListOwnedTasksQuery, IReadOnlyCollection<TaskDto>>, ListOwnedTasksQueryHandler>();
        services.AddScoped<ICommandHandler<GetOwnedTaskQuery, TaskDto>, GetOwnedTaskQueryHandler>();
    }
}
