using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.Update;
using BLAInterview.Domain.Tasks;
using FluentValidation;

namespace BLAInterview.Application.UnitTests.Tasks;

public class UpdateTaskCommandHandlerSpecs
{
    private static readonly DateTimeOffset SampleCreated = DateTimeOffset.Parse(
        "2020-01-15T00:00:00+00:00",
        System.Globalization.CultureInfo.InvariantCulture);

    [Fact]
    public async Task UpdateTaskCommandHandler_UpdatesTask_WhenCommandIsValid()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: "Updated title",
            Description: null,
            Priority: null,
            Status: "InProgress");
        IValidator<UpdateTaskCommand> validator = new UpdateTaskCommandValidator();
        var repository = new UpdateTestStub(
            new TaskDto(1, "Updated title", "idp-user-123", SampleCreated));
        ICommandHandler<UpdateTaskCommand, TaskDto> handler =
            new UpdateTaskCommandHandler(validator, repository);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Updated title", result.Value.Title);
        Assert.Equal(1, result.Value.Id);
    }

    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsUpdatedDescription_WhenCommandContainsDescription()
    {
        // Arrange: BE-API-006 / validator requires description length > 10 and < 50.
        const string newDescription = "A task description in required length here.";
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: null,
            Description: newDescription,
            Priority: null,
            Status: null);
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(
            new UpdateTaskCommandValidator(),
            new UpdateTestStub(
                new TaskDto(1, "Title", "idp-user-123", SampleCreated)));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, result.Value.Description);
    }

    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsUpdatedPriority_WhenCommandContainsPriority()
    {
        // Arrange
        const string newPriority = "High";
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: "Title",
            Description: null,
            Priority: newPriority,
            Status: null);
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(
            new UpdateTaskCommandValidator(),
            new UpdateTestStub(
                new TaskDto(1, "Title", "idp-user-123", SampleCreated)));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newPriority, result.Value.Priority);
    }

    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsUpdatedStatus_WhenCommandContainsStatus()
    {
        // Arrange
        const string newStatus = "Completed";
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: "Title",
            Description: null,
            Priority: null,
            Status: newStatus);
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(
            new UpdateTaskCommandValidator(),
            new UpdateTestStub(
                new TaskDto(1, "Title", "idp-user-123", SampleCreated)));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(newStatus, result.Value.Status);
    }

    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsNotFound_WhenTaskIsNotOwnedByCaller()
    {
        // Arrange
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-999",
            Title: "Title",
            Description: null,
            Priority: null,
            Status: "Pending");
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(
            new UpdateTaskCommandValidator(),
            new UpdateTestStub(null));

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
            Description: null,
            Priority: null,
            Status: "Bogus");
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(
            new UpdateTaskCommandValidator(),
            new UpdateTestStub(
                new TaskDto(1, "Title", "idp-user-123", SampleCreated))); // not reached when status is invalid

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("TASK_STATUS_INVALID", error.Metadata["Code"]);
    }

    private sealed class UpdateTestStub(TaskDto? updateResult) : ITaskRepository
    {
        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(
            string ownerId,
            CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken) => Task.FromResult(updateResult);
    }
}
