using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.List;
using BLAInterview.Domain.Tasks;

namespace BLAInterview.Application.UnitTests.Tasks;

public class ListOwnedTasksQueryHandlerSpecs
{
    [Fact]
    public async Task ListOwnedTasksQueryHandler_ReturnsOwnedTasks_WhenUserHasTasks()
    {
        // Arrange
        var ownedTask = new TaskDto(
            Id: 101,
            Title: "Prepare interview notes",
            OwnerId: "idp-user-123",
            Created: new DateTimeOffset(2026, 4, 29, 10, 15, 0, TimeSpan.Zero));
        var otherUsersTask = new TaskDto(
            Id: 202,
            Title: "Review scorecard",
            OwnerId: "idp-user-456",
            Created: new DateTimeOffset(2026, 4, 29, 10, 45, 0, TimeSpan.Zero));
        var repository = new StubTaskReadRepository([ownedTask, otherUsersTask]);
        var handler = new ListOwnedTasksQueryHandler(repository);
        var query = new ListOwnedTasksQuery("idp-user-123");

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        var task = Assert.Single(result.Value);
        Assert.Equal(101, task.Id);
        Assert.Equal("Prepare interview notes", task.Title);
        Assert.Equal("idp-user-123", task.OwnerId);
        Assert.Equal(new DateTimeOffset(2026, 4, 29, 10, 15, 0, TimeSpan.Zero), task.Created);
    }

    [Fact]
    public async Task ListOwnedTasksQueryHandler_ReturnsEmptyList_WhenUserHasNoTasks()
    {
        var repository = new StubTaskReadRepository(
        [
            new TaskDto(
                Id: 202,
                Title: "Review scorecard",
                OwnerId: "idp-user-456",
                Created: new DateTimeOffset(2026, 4, 29, 10, 45, 0, TimeSpan.Zero))
        ]);
        var handler = new ListOwnedTasksQueryHandler(repository);
        var query = new ListOwnedTasksQuery("idp-user-123");

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task ListOwnedTasksQueryHandler_ExcludesOtherUsersTasks_WhenUserRequestsList()
    {
        var ownedTask = new TaskDto(
            Id: 101,
            Title: "Prepare interview notes",
            OwnerId: "idp-user-123",
            Created: new DateTimeOffset(2026, 4, 29, 10, 15, 0, TimeSpan.Zero));
        var repository = new StubTaskReadRepository(
        [
            ownedTask,
            new TaskDto(
                Id: 202,
                Title: "Review scorecard",
                OwnerId: "idp-user-456",
                Created: new DateTimeOffset(2026, 4, 29, 10, 45, 0, TimeSpan.Zero))
        ]);
        var handler = new ListOwnedTasksQueryHandler(repository);
        var query = new ListOwnedTasksQuery("idp-user-123");

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var task = Assert.Single(result.Value);
        Assert.Equal(ownedTask, task);
    }

    private sealed class StubTaskReadRepository(IReadOnlyCollection<TaskDto> tasks) : ITaskRepository
    {
        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(
            string ownerId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<TaskDto> ownedTasks = tasks
                .Where(task => task.OwnerId == ownerId)
                .ToList();

            return Task.FromResult(ownedTasks);
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
