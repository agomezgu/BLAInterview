using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.Delete;
using BLAInterview.Domain.Tasks;

namespace BLAInterview.Application.UnitTests.Tasks;

/// <summary>
/// BE-API-007: <c>DeleteTaskCommand</c> + handler + <see cref="ITaskRepository"/> delete contract.
/// </summary>
public class DeleteTaskCommandHandlerSpecs
{
    [Fact]
    public async Task DeleteTaskCommandHandler_ReturnsSuccess_WhenUserDeletesOwnedTask()
    {
        var command = new DeleteTaskCommand(99, "idp-user-456");
        var repository = new DeleteOwnedSuccessStub(expectedTaskId: 99, expectedOwnerId: "idp-user-456");
        ICommandHandler<DeleteTaskCommand, bool> handler = new DeleteTaskCommandHandler(repository);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.Equal(1, repository.DeleteOwnedCallCount);
    }

    [Fact]
    public async Task DeleteTaskCommandHandler_ReturnsNotFound_WhenTaskOwnedByAnotherUser()
    {
        var command = new DeleteTaskCommand(55, "idp-caller-789");
        var repository = new DeleteOwnedNoRowStub();
        ICommandHandler<DeleteTaskCommand, bool> handler = new DeleteTaskCommandHandler(repository);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("Task not found for this user.", error.Message);
        Assert.Equal("TASK_NOT_FOUND", error.Metadata["Code"]);
        Assert.Equal(1, repository.DeleteOwnedCallCount);
    }

    [Fact]
    public async Task DeleteTaskCommandHandler_ReturnsNotFound_WhenTaskIdDoesNotExist()
    {
        var command = new DeleteTaskCommand(9_999_001, "idp-user-456");
        var repository = new DeleteOwnedNoRowStub();
        ICommandHandler<DeleteTaskCommand, bool> handler = new DeleteTaskCommandHandler(repository);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result.IsFailed);
        var error = Assert.Single(result.Errors);
        Assert.Equal("Task not found for this user.", error.Message);
        Assert.Equal("TASK_NOT_FOUND", error.Metadata["Code"]);
        Assert.Equal(1, repository.DeleteOwnedCallCount);
    }

    private sealed class DeleteOwnedSuccessStub(int expectedTaskId, string expectedOwnerId) : ITaskRepository
    {
        public int DeleteOwnedCallCount { get; private set; }

        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<TaskDto?> GetOwnedTaskByIdAsync(int taskId, string ownerId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken)
        {
            DeleteOwnedCallCount++;
            var success = taskId == expectedTaskId
                && string.Equals(ownerId, expectedOwnerId, StringComparison.Ordinal);
            return Task.FromResult(success);
        }
    }

    /// <summary>Simulates <c>DELETE</c> matching no row (wrong owner or missing id): repository returns <c>false</c>.</summary>
    private sealed class DeleteOwnedNoRowStub : ITaskRepository
    {
        public int DeleteOwnedCallCount { get; private set; }

        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<TaskDto?> GetOwnedTaskByIdAsync(int taskId, string ownerId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken)
        {
            DeleteOwnedCallCount++;
            return Task.FromResult(false);
        }
    }
}
