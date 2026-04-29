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
    public void Task_AssignsOwnerId_WhenTaskIsCreated()
    {
        Assert.Fail("RED: BE-API-003-T002 not implemented yet.");
    }

    [Fact]
    public void Task_RejectsCreation_WhenTitleIsMissing()
    {
        Assert.Fail("RED: BE-API-003-T003 not implemented yet.");
    }

    [Fact]
    public void Task_ExposesCreatedTaskDetails_WhenTaskIsCreated()
    {
        Assert.Fail("RED: BE-API-003-T004 not implemented yet.");
    }
}
