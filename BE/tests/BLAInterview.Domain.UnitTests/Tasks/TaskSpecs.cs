using DomainTask = BLAInterview.Domain.Tasks.Task;

namespace BLAInterview.Domain.UnitTests.Tasks;

public class TaskSpecs
{
    [Fact]
    public void Task_CreatesTask_WhenTitleAndOwnerAreProvided()
    {
        // Arrange
        var title = "Prepare interview notes";
        var ownerId = "idp-user-123";

        // Act
        var task = DomainTask.Create(title, ownerId);

        // Assert
        Assert.NotNull(task);
        Assert.Equal(title, task.Title);
        Assert.Equal(ownerId, task.OwnerId);
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
