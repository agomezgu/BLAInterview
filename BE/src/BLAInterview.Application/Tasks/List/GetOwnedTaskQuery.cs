using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using FluentResults;

namespace BLAInterview.Application.Tasks.List;

/// <summary>
/// Gets a single task by id, scoped to the specified owner.
/// </summary>
public sealed record GetOwnedTaskQuery(int TaskId, string OwnerId) : ICommand<TaskDto>;

public sealed class GetOwnedTaskQueryHandler(
    ITaskRepository repository) : ICommandHandler<GetOwnedTaskQuery, TaskDto>
{
    public async Task<Result<TaskDto>> HandleAsync(
        GetOwnedTaskQuery command,
        CancellationToken cancellationToken)
    {
        var task = await repository.GetOwnedTaskByIdAsync(
            command.TaskId,
            command.OwnerId,
            cancellationToken);

        if (task is null)
        {
            return Result.Fail<TaskDto>(
                new Error("Task not found for this user.")
                    .WithMetadata("Code", "TASK_NOT_FOUND"));
        }

        return Result.Ok(task);
    }
}
