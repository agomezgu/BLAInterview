using Xunit;

namespace BLAInterview.Domain.UnitTests.Tasks;

/// <summary>
/// Domain invariants for mutable task fields when an update is applied to <see cref="BLAInterview.Domain.Tasks.TaskEntity"/>.
/// </summary>
public class TaskEntityUpdateSpecs
{
    [Fact]
    public void TaskEntity_AllowsStatusChange_WhenNewStatusIsAllowed()
    {
        // RED: BE-API-006-T008 not implemented yet.
    }

    [Fact]
    public void TaskEntity_RejectsInvalidStatus_WhenUpdateProvidesDisallowedValue()
    {
        // RED: BE-API-006-T009 not implemented yet.
    }
}
