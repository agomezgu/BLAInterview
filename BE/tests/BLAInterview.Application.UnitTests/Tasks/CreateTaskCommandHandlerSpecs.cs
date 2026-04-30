using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;

namespace BLAInterview.Application.UnitTests.Tasks;

public class CreateTaskCommandHandlerSpecs
{
    [Fact]
    public async Task CreateTaskCommandHandler_CreatesTask_WhenCommandHasTitleAndOwner()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Prepare interview notes",
            OwnerId: "idp-user-123");
        FluentValidation.IValidator<CreateTaskCommand> validator =
            new CreateTaskCommandValidator();
        var repository = new StubTaskRepository(42);
        ICommandHandler<CreateTaskCommand, int> handler =
            new CreateTaskCommandHandler(validator, repository);

        // Act
        FluentResults.Result<int> result =
            await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.Equal(42, result.Value);
        Assert.NotNull(repository.StoredTask);
        Assert.Equal("Prepare interview notes", repository.StoredTask.Title);
        Assert.Equal("idp-user-123", repository.StoredTask.OwnerId);
    }

    [Fact]
    public async Task CreateTaskCommandHandler_AssignsOwnerId_WhenTaskIsStored()
    {
        var command = new CreateTaskCommand(
            Title: "Prepare interview notes",
            OwnerId: "idp-user-123");
        var repository = new StubTaskRepository(42);
        ICommandHandler<CreateTaskCommand, int> handler =
            new CreateTaskCommandHandler(new CreateTaskCommandValidator(), repository);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(repository.StoredTask);
        Assert.Equal("idp-user-123", repository.StoredTask.OwnerId);
    }

    [Fact]
    public async Task CreateTaskCommandHandler_ReturnsValidationFailure_WhenTitleIsMissing()
    {
        var command = new CreateTaskCommand(
            Title: "",
            OwnerId: "idp-user-123");
        var repository = new StubTaskRepository(42);
        ICommandHandler<CreateTaskCommand, int> handler =
            new CreateTaskCommandHandler(new CreateTaskCommandValidator(), repository);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("TASK_TITLE_REQUIRED", error.Metadata["Code"]);
        Assert.Null(repository.StoredTask);
    }

    [Fact]
    public async Task CreateTaskCommandHandler_ReturnsCreatedTaskDetails_WhenTaskIsCreated()
    {
        var command = new CreateTaskCommand(
            Title: "Prepare interview notes",
            OwnerId: "idp-user-123");
        var repository = new StubTaskRepository(42);
        ICommandHandler<CreateTaskCommand, int> handler =
            new CreateTaskCommandHandler(new CreateTaskCommandValidator(), repository);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
        Assert.NotNull(repository.StoredTask);
        Assert.Equal(42, repository.StoredTask.Id);
        Assert.Equal("Prepare interview notes", repository.StoredTask.Title);
        Assert.NotEqual(default, repository.StoredTask.Created);
    }

    private sealed class StubTaskRepository(int taskId) : ITaskRepository
    {
        public TaskEntity? StoredTask { get; private set; }

        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken)
        {
            StoredTask = task;
            StoredTask.Id = taskId;

            return Task.FromResult(taskId);
        }

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken) =>
            throw new NotImplementedException();
    }
}
