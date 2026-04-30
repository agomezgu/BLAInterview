using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.Update;
using FluentValidation;

namespace BLAInterview.Application.UnitTests.Tasks;

public class UpdateTaskCommandHandlerSpecs
{
    [Fact]
    public async Task UpdateTaskCommandHandler_UpdatesTask_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: "Updated title",
            Status: "InProgress");
        IValidator<UpdateTaskCommand> validator = new UpdateTaskCommandValidator();
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(validator);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated title", result.Value.Title);
        Assert.Equal(1, result.Value.Id);
    }

    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsNotFound_WhenTaskIsNotOwnedByCaller()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-999",
            Title: "Title",
            Status: "Pending");
        ICommandHandler<UpdateTaskCommand, TaskDto> handler =
            new UpdateTaskCommandHandler(new UpdateTaskCommandValidator());

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("TASK_NOT_FOUND", error.Metadata["Code"]);
    }

    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsValidationFailure_WhenStatusIsInvalid()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: "Title",
            Status: "Bogus");
        ICommandHandler<UpdateTaskCommand, TaskDto> handler =
            new UpdateTaskCommandHandler(new UpdateTaskCommandValidator());

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("TASK_STATUS_INVALID", error.Metadata["Code"]);
    }
}
