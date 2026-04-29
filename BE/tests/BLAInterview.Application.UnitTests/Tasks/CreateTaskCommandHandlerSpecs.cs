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
    public void CreateTaskCommandHandler_AssignsOwnerId_WhenTaskIsStored()
    {
        Assert.Fail("RED: BE-API-003-T002 not implemented yet.");
    }

    [Fact]
    public void CreateTaskCommandHandler_ReturnsValidationFailure_WhenTitleIsMissing()
    {
        Assert.Fail("RED: BE-API-003-T003 not implemented yet.");
    }

    [Fact]
    public void CreateTaskCommandHandler_ReturnsCreatedTaskDetails_WhenTaskIsCreated()
    {
        Assert.Fail("RED: BE-API-003-T004 not implemented yet.");
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
    }
}
