using BLAInterview.Application.Abstractions;
using FluentResults;

namespace BLAInterview.Application.Tasks.Delete;

public sealed record DeleteTaskCommand(int TaskId, string OwnerId) : ICommand<bool>;

public sealed class DeleteTaskCommandHandler(ITaskRepository taskRepository)
    : ICommandHandler<DeleteTaskCommand, bool>
{
    public async Task<Result<bool>> HandleAsync(
        DeleteTaskCommand command,
        CancellationToken cancellationToken)
    {
        var deleted = await taskRepository.DeleteOwnedTaskAsync(
            command.TaskId,
            command.OwnerId,
            cancellationToken);

        if (!deleted)
        {
            return Result.Fail<bool>(
                new Error("Task not found for this user.")
                    .WithMetadata("Code", "TASK_NOT_FOUND"));
        }

        return Result.Ok(true);
    }
}
