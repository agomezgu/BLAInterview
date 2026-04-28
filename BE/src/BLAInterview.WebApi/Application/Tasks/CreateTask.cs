using FluentResults;
using FluentValidation;
using BLAInterview.WebApi.Application.Abstractions;

namespace BLAInterview.WebApi.Application.Tasks;

public sealed record CreateTaskCommand(string Title, string OwnerId) : ICommand<CreateTaskResult>;

public sealed record CreateTaskResult(Guid Id, string Title, string OwnerId);

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(command => command.Title)
            .NotEmpty()
            .WithErrorCode("TASK_TITLE_REQUIRED")
            .WithMessage("Title is required.");

        RuleFor(command => command.OwnerId)
            .NotEmpty()
            .WithErrorCode("TASK_OWNER_REQUIRED")
            .WithMessage("OwnerId is required.");
    }
}

public sealed class CreateTaskCommandHandler(IValidator<CreateTaskCommand> validator)
    : ICommandHandler<CreateTaskCommand, CreateTaskResult>
{
    public async Task<Result<CreateTaskResult>> HandleAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result.Fail<CreateTaskResult>(
                validationResult.Errors.Select(failure =>
                    new Error(failure.ErrorMessage).WithMetadata("Code", failure.ErrorCode)));
        }

        var task = new CreateTaskResult(Guid.NewGuid(), command.Title, command.OwnerId);

        return Result.Ok(task);
    }
}
