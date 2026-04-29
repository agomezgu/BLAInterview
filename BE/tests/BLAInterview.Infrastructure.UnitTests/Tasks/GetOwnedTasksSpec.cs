using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.Tasks;

namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task repository read behavior against the PostgreSQL fixture.
/// </summary>
public class GetOwnedTasksSpecs
{
    [Fact]
    public async Task TaskRepository_ReturnsOwnedTasks_WhenUserHasTasks()
    {
        // Arrange
        var fixture = new Fixtures.PostgresFixture();
        await fixture.InitializeAsync();
        try
        {
            var repository = new TaskRepository(fixture.DataSource);
            var ownedTask = TaskEntity.Create(
                "Prepare interview notes",
                "idp-user-123");
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
        finally
        {
            await fixture.DisposeAsync();
        }
    }

    [Fact]
    public void TaskRepository_ReturnsEmptyList_WhenUserHasNoTasks()
    {
        Assert.Fail("RED: BE-API-004-T002 not implemented yet.");
    }

    [Fact]
    public void TaskRepository_ExcludesOtherUsersTasks_WhenUserRequestsList()
    {
        Assert.Fail("RED: BE-API-004-T003 not implemented yet.");
    }
}
