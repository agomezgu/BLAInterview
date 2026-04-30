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

    /// <summary>
    /// BE-API-006: status change must follow <see cref="TaskEntity"/>'s allowed transitions (e.g. not Pending to Completed);
    /// the handler should load the current task (e.g. via <see cref="ITaskRepository.GetOwnedTasksAsync" />) and
    /// reject with <c>TASK_STATUS_TRANSITION_INVALID</c> before a disallowed update.
    /// </summary>
    [Fact]
    public async Task UpdateTaskCommandHandler_ReturnsValidationFailure_WhenStatusTransitionIsDisallowed()
    {
        // Arrange: persisted task is Pending; requested transition Pending -> Completed is not in the allowed set.
        var existing = new TaskDto(1, "Title", "idp-user-123", SampleCreated, null, null, "Pending");
        var command = new UpdateTaskCommand(
            TaskId: 1,
            OwnerId: "idp-user-123",
            Title: null,
            Description: null,
            Priority: null,
            Status: "Completed");
        ICommandHandler<UpdateTaskCommand, TaskDto> handler = new UpdateTaskCommandHandler(
            new UpdateTaskCommandValidator(),
            new GetOwnedAndUpdateTestStub(owned: [existing], onUpdate: existing with { Status = "Completed" }));

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("TASK_STATUS_TRANSITION_INVALID", error.Metadata["Code"]);
    }

    private sealed class UpdateTestStub(TaskDto? updateResult) : ITaskRepository
    {
        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(
            string ownerId,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<TaskDto>>(
                updateResult is not null && string.Equals(updateResult.OwnerId, ownerId, StringComparison.Ordinal)
                    ? (IReadOnlyCollection<TaskDto>)[updateResult]
                    : Array.Empty<TaskDto>());

        public Task<TaskDto?> GetOwnedTaskByIdAsync(int taskId, string ownerId, CancellationToken cancellationToken) =>
            Task.FromResult(
                updateResult is not null
                && updateResult.Id == taskId
                && string.Equals(updateResult.OwnerId, ownerId, StringComparison.Ordinal)
                    ? updateResult
                    : null);

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken) => Task.FromResult(updateResult);

        public Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken) =>
            Task.FromResult(false);
    }

    /// <summary>
    /// Exposes <see cref="ITaskRepository.GetOwnedTasksAsync"/> for specs that will drive status-transition
    /// validation in the handler, plus an optional <see cref="ITaskRepository.UpdateOwnedTaskAsync"/> return value.
    /// </summary>
    private sealed class GetOwnedAndUpdateTestStub(
        IReadOnlyList<TaskDto> owned,
        TaskDto? onUpdate) : ITaskRepository
    {
        public int UpdateCallCount { get; private set; }

        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(
            string ownerId,
            CancellationToken cancellationToken) =>
            Task.FromResult<IReadOnlyCollection<TaskDto>>(owned);

        public Task<TaskDto?> GetOwnedTaskByIdAsync(int taskId, string ownerId, CancellationToken cancellationToken) =>
            Task.FromResult(
                (TaskDto?)owned.FirstOrDefault(t => t.Id == taskId && t.OwnerId == ownerId));

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken)
        {
            UpdateCallCount++;
            if (onUpdate is not null
                && string.Equals(ownerId, onUpdate.OwnerId, StringComparison.Ordinal)
                && taskId == onUpdate.Id)
            {
                return Task.FromResult<TaskDto?>(onUpdate);
            }

            return Task.FromResult(
                (TaskDto?)owned.FirstOrDefault(t => t.Id == taskId && t.OwnerId == ownerId));
        }

        public Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken) =>
            Task.FromResult(false);
    }
}
