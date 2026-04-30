using System.Linq;
using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using FluentResults;
using FluentValidation;

namespace BLAInterview.Application.Tasks.Update;

public sealed record UpdateTaskCommand(
    int TaskId,
    string OwnerId,
    string? Title,
    string? Status) : ICommand<TaskDto>;

public sealed class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(c => c.TaskId)
            .GreaterThan(0)
            .WithErrorCode("TASK_ID_INVALID")
            .WithMessage("TaskId must be a positive value.");

        RuleFor(c => c.OwnerId)
            .NotEmpty()
            .WithErrorCode("TASK_OWNER_REQUIRED")
            .WithMessage("Owner is required.");

        When(
            c => c.Title is not null,
            () => RuleFor(c => c.Title!)
                .NotEmpty()
                .WithErrorCode("TASK_TITLE_REQUIRED")
                .WithMessage("Title cannot be empty."));

        When(
            c => c.Status is not null,
            () => RuleFor(c => c.Status!)
                .Must(BeValidStatus)
                .WithErrorCode("TASK_STATUS_INVALID")
                .WithMessage("Task status is invalid."));
    }

    private static bool BeValidStatus(string status) =>
        status is "Pending" or "InProgress" or "Completed" or "Cancelled";
}

public sealed class UpdateTaskCommandHandler(
    IValidator<UpdateTaskCommand> validator) : ICommandHandler<UpdateTaskCommand, TaskDto>
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

        // Red phase: use case is not yet implemented; Green phase will load, apply changes, and persist.
        return Result.Fail<TaskDto>(
            new Error("Update task is not yet implemented (BE-API-006).")
                .WithMetadata("Code", "RED_UPDATE"));
    }
}
