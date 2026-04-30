using FluentValidation;

namespace BLAInterview.Application.Tasks.Update;

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
            c => c.Description is not null,
            () => RuleFor(c => c.Description!)
                .Must(BeValidDescriptionLength)
                .WithErrorCode("TASK_DESCRIPTION_INVALID")
                .WithMessage("Task description must be more than 10 and less than 50 characters."));

        When(
            c => c.Priority is not null,
            () => RuleFor(c => c.Priority!)
                .Must(BeValidPriority)
                .WithErrorCode("TASK_PRIORITY_INVALID")
                .WithMessage("Task priority is invalid."));

        When(
            c => c.Status is not null,
            () => RuleFor(c => c.Status!)
                .Must(BeValidStatus)
                .WithErrorCode("TASK_STATUS_INVALID")
                .WithMessage("Task status is invalid."));
    }

    private static bool BeValidDescriptionLength(string description) =>
        description.Length > 10 && description.Length < 50;

    private static bool BeValidPriority(string priority) =>
        priority is "High" or "Medium" or "Low";

    private static bool BeValidStatus(string status) =>
        status is "Pending" or "InProgress" or "Completed" or "Cancelled";
}
