using BLAInterview.Application.Abstractions;
using FluentResults;
using FluentValidation;

namespace BLAInterview.Application.Tasks.Create;

public sealed record CreateTaskCommand(string Title, string OwnerId) : ICommand<int>;

public sealed class CreateTaskCommandHandler(IValidator<CreateTaskCommand> validator)
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

        var taskId = 1;

        return Result.Ok(taskId);
    }
}
