using FluentValidation;

namespace BLAInterview.Application.Tasks.Create ;

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
