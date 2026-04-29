using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.UnitTests.Fixtures;
using BLAInterview.Infrastructure.Tasks;

namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task repository read behavior against the PostgreSQL fixture.
/// </summary>
public class GetOwnedTasksSpecs : IClassFixture<LocalPostgresFixture>, IAsyncLifetime
{
    private readonly LocalPostgresFixture _fixture;

    public GetOwnedTasksSpecs(LocalPostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => _fixture.ResetAsync();

    [Fact]
    public async Task TaskRepository_ReturnsOwnedTasks_WhenUserHasTasks()
    {
        // Arrange
        var repository = new TaskRepository(_fixture.DataSource);
        var ownedTask = TaskEntity.Create(
            "Prepare interview notes",
            "idp-user-123",
            null);
        await repository.AddAsync(ownedTask, CancellationToken.None);

        // Act
        var tasks = await repository.GetOwnedTasksAsync("idp-user-123", CancellationToken.None);

        // Assert
        var task = Assert.Single(tasks);
        Assert.Equal(ownedTask.Id, task.Id);
        Assert.Equal("Prepare interview notes", task.Title);
        Assert.Equal("idp-user-123", task.OwnerId);
        Assert.NotEqual(default, task.Created);
    }

    [Fact]
    public async Task TaskRepository_ReturnsEmptyList_WhenUserHasNoTasks()
    {
        var repository = new TaskRepository(_fixture.DataSource);

        var tasks = await repository.GetOwnedTasksAsync("idp-user-123", CancellationToken.None);

        Assert.Empty(tasks);
    }

    [Fact]
    public async Task TaskRepository_ExcludesOtherUsersTasks_WhenUserRequestsList()
    {
        var repository = new TaskRepository(_fixture.DataSource);
        var ownedTask = TaskEntity.Create(
            "Prepare interview notes",
            "idp-user-123",
            null);
        var otherUsersTask = TaskEntity.Create(
            "Review scorecard",
            "idp-user-456",
            null);
        await repository.AddAsync(ownedTask, CancellationToken.None);
        await repository.AddAsync(otherUsersTask, CancellationToken.None);

        var tasks = await repository.GetOwnedTasksAsync("idp-user-123", CancellationToken.None);

        var task = Assert.Single(tasks);
        Assert.Equal(ownedTask.Id, task.Id);
        Assert.Equal("idp-user-123", task.OwnerId);
    }
}
