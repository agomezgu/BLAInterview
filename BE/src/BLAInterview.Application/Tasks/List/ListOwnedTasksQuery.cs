using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using FluentResults;

namespace BLAInterview.Application.Tasks.List;

/// <summary>
/// Lists tasks owned by the specified owner.
/// </summary>
public sealed record ListOwnedTasksQuery(string OwnerId) : ICommand<IReadOnlyCollection<TaskDto>>;

public sealed class ListOwnedTasksQueryHandler(
    ITaskRepository repository)
    : ICommandHandler<ListOwnedTasksQuery, IReadOnlyCollection<TaskDto>>
{
    public async Task<Result<IReadOnlyCollection<TaskDto>>> HandleAsync(
        ListOwnedTasksQuery command,
        CancellationToken cancellationToken)
    {
        var ownedTasks = await repository.GetOwnedTasksAsync(command.OwnerId, cancellationToken);

        return Result.Ok(ownedTasks);
    }
}
