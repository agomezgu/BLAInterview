using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.Tasks;
using BLAInterview.Infrastructure.UnitTests.Fixtures;

namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task update persistence against the PostgreSQL fixture.
/// </summary>
public class UpdateTaskSpecs : IClassFixture<LocalPostgresFixture>, IAsyncLifetime
{
    private readonly LocalPostgresFixture _fixture;
    private readonly TaskRepository _taskRepository;

    public UpdateTaskSpecs(LocalPostgresFixture fixture)
    {
        _fixture = fixture;
        _taskRepository = new TaskRepository(fixture.DataSource);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => _fixture.ResetAsync();

    [Fact]
    public async Task TaskRepository_PersistsUpdatedFields_WhenOwnerUpdatesTask()
    {
        // Arrange
        var id = await _taskRepository.AddAsync(
            TaskEntity.Create("Original", "owner-1", null),
            CancellationToken.None);

        // Act
        // (Red) repository will expose an update path; until then, title remains the original.
        // Assert
        var tasks = await _taskRepository.GetOwnedTasksAsync("owner-1", CancellationToken.None);
        var task = Assert.Single(tasks, t => t.Id == id);
        Assert.Equal("Updated", task.Title);
    }

    [Fact]
    public async Task TaskRepository_DoesNotExposeOtherUserTask_WhenUpdateTargetsWrongOwner()
    {
        // Arrange
        const string ownerA = "owner-a";
        const string ownerB = "owner-b";
        var idA = await _taskRepository.AddAsync(
            TaskEntity.Create("A task", ownerA, null),
            CancellationToken.None);

        // Act
        var listB = await _taskRepository.GetOwnedTasksAsync(ownerB, CancellationToken.None);
        // Assert
        Assert.DoesNotContain(idA, listB.Select(t => t.Id));
        // Red: cross-owner update must be rejected; this placeholder fails until update ownership is implemented.
        Assert.Empty(await _taskRepository.GetOwnedTasksAsync(ownerA, CancellationToken.None));
    }
}
