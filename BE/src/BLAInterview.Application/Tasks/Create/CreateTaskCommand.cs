using BLAInterview.Application.Abstractions;
using BLAInterview.Domain.Tasks;
using FluentResults;
using FluentValidation;

namespace BLAInterview.Application.Tasks.Create;

public sealed record CreateTaskCommand(string Title, string OwnerId) : ICommand<int>;

public sealed class CreateTaskCommandHandler(
    IValidator<CreateTaskCommand> validator,
    ITaskRepository repository)
    : ICommandHandler<CreateTaskCommand, int>
{
    public async Task<Result<int>> HandleAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<int>(
                validationResult.Errors.Select(failure =>
                    new Error(failure.ErrorMessage).WithMetadata("Code", failure.ErrorCode)));
        }

        var task = TaskEntity.Create(command.Title, command.OwnerId, null);
        var taskId = await repository.AddAsync(task, cancellationToken);

        return Result.Ok(taskId);
    }
}
