using BLAInterview.Domain.Tasks;
using TaskStatus = BLAInterview.Domain.ValueObjects.Task.TaskStatus;

namespace BLAInterview.Domain.UnitTests.Tasks;

/// <summary>
/// Domain invariants for mutable task fields when an update is applied to <see cref="TaskEntity"/>.
/// </summary>
public class TaskEntityUpdateSpecs
{
    [Fact]
    public void TaskEntity_AllowsStatusChange_WhenNewStatusIsAllowed()
    {
        // Arrange
        var entity = TaskEntity.Create("T", "owner", status: new TaskStatus("Pending"));
        // Act
        TaskEntity? afterPlannedStatusChange = null; // Red: will become a domain method that transitions status (BE-API-006).
        // Assert
        Assert.NotNull(afterPlannedStatusChange);
        Assert.Equal("InProgress", afterPlannedStatusChange.Status?.Value);
    }

    [Fact]
    public void TaskEntity_RejectsInvalidStatus_WhenUpdateProvidesDisallowedValue()
    {
        // Arrange
        var entity = TaskEntity.Create("T", "owner", status: new TaskStatus("Pending"));
        // Act
        // (Red) invalid status is rejected on the update path, not on the existing value.
        // Assert
        Assert.Equal("Bogus", entity.Status?.Value);
    }
}
