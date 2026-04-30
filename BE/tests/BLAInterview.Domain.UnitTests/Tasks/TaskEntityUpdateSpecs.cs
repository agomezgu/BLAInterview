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
        TaskEntity? afterPlannedStatusChange = entity.TransitionTo(new TaskStatus("InProgress"));
        // Assert
        Assert.NotNull(afterPlannedStatusChange);
        Assert.Equal("InProgress", afterPlannedStatusChange.Status?.Value);
    }

    [Fact]
    public void TaskEntity_RejectsInvalidStatus_WhenUpdateProvidesDisallowedValue()
    {
        // Arrange
        var entity = TaskEntity.Create("T", "owner", status: new TaskStatus("Pending"));
        // Act / Assert: invalid value fails before the entity is mutated; existing status stays valid.
        Assert.Throws<ArgumentException>(() => entity.ApplyStatusUpdate("Bogus"));
        Assert.Equal("Pending", entity.Status?.Value);
    }

    [Fact]
    public void TaskEntity_Throws_WhenStatusUpdateIsNotAllowedByTransition()
    {
        // Arrange: Pending -> Completed is not allowed in IsTransitionAllowed
        var entity = TaskEntity.Create("T", "owner", status: new TaskStatus("Pending"));

        // Act / Assert: transition is rejected; status is unchanged
        Assert.Throws<InvalidOperationException>(() => entity.ApplyStatusUpdate("Completed"));
        Assert.Equal("Pending", entity.Status?.Value);
    }
}
