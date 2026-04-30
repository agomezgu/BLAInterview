using Xunit;

namespace BLAInterview.WebApi.UnitTests.Tasks;

public class TaskUpdateEndpointSpecs
{
    [Fact]
    public void TaskUpdateEndpoint_UpdatesEditableFields_WhenUserOwnsTask()
    {
        // RED: BE-API-006-T001 not implemented yet.
    }

    [Fact]
    public void TaskUpdateEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser()
    {
        // RED: BE-API-006-T002 not implemented yet.
    }

    [Fact]
    public void TaskUpdateEndpoint_ReturnsValidationFailure_WhenStatusIsInvalid()
    {
        // RED: BE-API-006-T003 not implemented yet.
    }

    [Fact]
    public void TaskUpdateEndpoint_ReturnsUpdatedTaskState_WhenUpdateSucceeds()
    {
        // RED: BE-API-006-T004 not implemented yet.
    }
}
