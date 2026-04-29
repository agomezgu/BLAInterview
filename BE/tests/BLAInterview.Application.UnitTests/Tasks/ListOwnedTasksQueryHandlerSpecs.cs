using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.List;

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
    public void ListOwnedTasksQueryHandler_ReturnsEmptyList_WhenUserHasNoTasks()
    {
        Assert.Fail("RED: BE-API-004-T002 not implemented yet.");
    }

    [Fact]
    public void ListOwnedTasksQueryHandler_ExcludesOtherUsersTasks_WhenUserRequestsList()
    {
        Assert.Fail("RED: BE-API-004-T003 not implemented yet.");
    }

    private sealed class StubTaskReadRepository(IReadOnlyCollection<TaskDto> tasks) : ITaskReadRepository
    {
        public Task<IReadOnlyCollection<TaskDto>> ListOwnedAsync(
            string ownerId,
            CancellationToken cancellationToken)
        {
            IReadOnlyCollection<TaskDto> ownedTasks = tasks
                .Where(task => task.OwnerId == ownerId)
                .ToList();

            return Task.FromResult(ownedTasks);
        }
    }
}
