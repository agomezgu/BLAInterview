namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task repository read behavior against the PostgreSQL fixture.
/// </summary>
public class GetOwnedTasksSpecs
{
    [Fact]
    public void TaskRepository_ReturnsOwnedTasks_WhenUserHasTasks()
    {
        Assert.Fail("RED: BE-API-004-T001 not implemented yet.");
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
