using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.Tasks;
using BLAInterview.Infrastructure.UnitTests.Fixtures;

namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task update persistence against the PostgreSQL fixture.
/// <para>
/// BE-API-006: the status update TDD test list (skipped, empty bodies) lives at the end of this class; remove <c>Fact(Skip=...)</c> and implement when going green.
/// </para>
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
    public async Task TaskRepository_PersistsTitle_WhenOwnerUpdatesTitle()
    {
        // Arrange
        var id = await _taskRepository.AddAsync(
            TaskEntity.Create("Original", "owner-1", null),
            CancellationToken.None);

        // Act
        var updated = await _taskRepository.UpdateOwnedTaskAsync(
            id,
            "owner-1",
            "Updated",
            null,
            null,
            null,
            CancellationToken.None);
        // Assert
        Assert.NotNull(updated);
        var tasks = await _taskRepository.GetOwnedTasksAsync("owner-1", CancellationToken.None);
        var task = Assert.Single(tasks, t => t.Id == id);
        Assert.Equal("Updated", task.Title);
    }

    [Fact]
    public async Task TaskRepository_PersistsDescription_WhenOwnerUpdatesDescription()
    {
        // Arrange: description length is validated in Application (BE-API-006).
        const string newDescription = "A task description in required length here.";
        var id = await _taskRepository.AddAsync(
            TaskEntity.Create("A title here", "owner-1", null),
            CancellationToken.None);

        // Act
        var updated = await _taskRepository.UpdateOwnedTaskAsync(
            id,
            "owner-1",
            null,
            newDescription,
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(newDescription, updated.Description);
    }

    [Fact]
    public async Task TaskRepository_PersistsPriority_WhenOwnerUpdatesPriority()
    {
        // Arrange
        const string newPriority = "High";
        var id = await _taskRepository.AddAsync(
            TaskEntity.Create("A title for priority test", "owner-1", null),
            CancellationToken.None);

        // Act
        var updated = await _taskRepository.UpdateOwnedTaskAsync(
            id,
            "owner-1",
            null,
            null,
            newPriority,
            null,
            CancellationToken.None);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(newPriority, updated.Priority);
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
        var crossOwnerResult = await _taskRepository.UpdateOwnedTaskAsync(
            idA,
            ownerB,
            "Hijacked",
            null,
            null,
            null,
            CancellationToken.None);
        // Assert
        Assert.Null(crossOwnerResult);
        var listA = await _taskRepository.GetOwnedTasksAsync(ownerA, CancellationToken.None);
        var taskA = Assert.Single(listA, t => t.Id == idA);
        Assert.Equal("A task", taskA.Title);
    }

}
