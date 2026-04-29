using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.UnitTests.Fixtures;
using BLAInterview.Infrastructure.Tasks;

namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task repository persistence behavior against the PostgreSQL fixture.
/// </summary>
public class CreateTaskSpecs : IClassFixture<LocalPostgresFixture>, IAsyncLifetime
{
    private readonly LocalPostgresFixture _fixture;
    private readonly TaskRepository _taskRepository;

    public CreateTaskSpecs(LocalPostgresFixture fixture)
    {
        _fixture = fixture;
        _taskRepository = new TaskRepository(fixture.DataSource);
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => _fixture.ResetAsync();

    [Fact]
    public async Task Task_CreatesTask_WhenTitleAndOwnerAreProvided()
    {
        // Arrange
        var taskEntity = TaskEntity.Create("Prepare interview notes", "idp-user-123", null);
        // Act
        var task = await _taskRepository.AddAsync(taskEntity, CancellationToken.None);

        // Assert
        Assert.True(task > 0);
        Assert.Equal(task, taskEntity.Id);
    }

    [Fact]
    public async Task Task_AssignsOwnerId_WhenTaskIsCreated()
    {
        var taskEntity = TaskEntity.Create("Prepare interview notes", "idp-user-123", null);

        await _taskRepository.AddAsync(taskEntity, CancellationToken.None);
        var tasks = await _taskRepository.GetOwnedTasksAsync("idp-user-123", CancellationToken.None);

        var task = Assert.Single(tasks);
        Assert.Equal("idp-user-123", task.OwnerId);
    }

    [Fact]
    public async Task Task_RejectsCreation_WhenTitleIsMissing()
    {
        var taskEntity = TaskEntity.Create(null!, "idp-user-123", null);

        await Assert.ThrowsAnyAsync<Exception>(() =>
            _taskRepository.AddAsync(taskEntity, CancellationToken.None));
    }

    [Fact]
    public async Task Task_ExposesCreatedTaskDetails_WhenTaskIsCreated()
    {
        var taskEntity = TaskEntity.Create("Prepare interview notes", "idp-user-123", null);

        var taskId = await _taskRepository.AddAsync(taskEntity, CancellationToken.None);
        var tasks = await _taskRepository.GetOwnedTasksAsync("idp-user-123", CancellationToken.None);

        var task = Assert.Single(tasks);
        Assert.Equal(taskId, task.Id);
        Assert.Equal("Prepare interview notes", task.Title);
        Assert.Equal("idp-user-123", task.OwnerId);
        Assert.NotEqual(default, task.Created);
    }
}
