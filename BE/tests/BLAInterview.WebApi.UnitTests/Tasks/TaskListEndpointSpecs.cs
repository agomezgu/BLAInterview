namespace BLAInterview.WebApi.UnitTests.Tasks;

public class TaskListEndpointSpecs
{
    [Fact]
    public void TaskListEndpoint_ReturnsOwnedTasks_WhenAuthenticatedUserHasTasks()
    {
        Assert.Fail("RED: BE-API-004-T001 not implemented yet.");
    }

    [Fact]
    public void TaskListEndpoint_ReturnsEmptyList_WhenAuthenticatedUserHasNoTasks()
    {
        Assert.Fail("RED: BE-API-004-T002 not implemented yet.");
    }

    [Fact]
    public void TaskListEndpoint_ExcludesOtherUsersTasks_WhenAuthenticatedUserRequestsList()
    {
        Assert.Fail("RED: BE-API-004-T003 not implemented yet.");
    }

    [Fact]
    public void TaskListEndpoint_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        Assert.Fail("RED: BE-API-004-T004 not implemented yet.");
    }
}
