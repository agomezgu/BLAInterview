using System.Linq;
using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;
using FluentResults;
using FluentValidation;

namespace BLAInterview.Application.Tasks.Update;

public sealed record UpdateTaskCommand(
    int TaskId,
    string OwnerId,
    string? Title,
    string? Description,
    string? Priority,
    string? Status) : ICommand<TaskDto>;

public sealed class UpdateTaskCommandHandler(
    IValidator<UpdateTaskCommand> validator,
    ITaskRepository taskRepository) : ICommandHandler<UpdateTaskCommand, TaskDto>
{
    public async Task<Result<TaskDto>> HandleAsync(
        UpdateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Fail<TaskDto>(
                validationResult.Errors.Select(
                    failure =>
                        new Error(failure.ErrorMessage)
                            .WithMetadata("Code", failure.ErrorCode)));
        }

        if (command.Status is not null)
        {
            var owned = await taskRepository.GetOwnedTasksAsync(command.OwnerId, cancellationToken);
            var current = owned.FirstOrDefault(t => t.Id == command.TaskId);
            if (current is null)
            {
                return Result.Fail<TaskDto>(
                    new Error("Task not found for this user.")
                        .WithMetadata("Code", "TASK_NOT_FOUND"));
            }

            if (!TaskEntity.IsStatusTransitionAllowed(current.Status, command.Status))
            {
                return Result.Fail<TaskDto>(
                    new Error("This status change is not allowed for the current task state.")
                        .WithMetadata("Code", "TASK_STATUS_TRANSITION_INVALID"));
            }
        }

        var updated = await taskRepository.UpdateOwnedTaskAsync(
            command.TaskId,
            command.OwnerId,
            command.Title,
            command.Description,
            command.Priority,
            command.Status,
            cancellationToken);

        if (updated is null)
        {
            return Result.Fail<TaskDto>(
                new Error("Task not found for this user.")
                    .WithMetadata("Code", "TASK_NOT_FOUND"));
        }

        var resultDto = updated with
        {
            Title = command.Title ?? updated.Title,
            Description = command.Description ?? updated.Description,
            Priority = command.Priority ?? updated.Priority,
            Status = command.Status ?? updated.Status
        };
        return Result.Ok(resultDto);
    }
}
