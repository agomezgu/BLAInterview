using BLAInterview.Domain.Tasks;
using BLAInterview.Domain.ValueObjects.Task;

namespace BLAInterview.Domain.UnitTests.Tasks;

public class TaskPrioritySpecs
{
    [Theory]
    [InlineData("High")]
    [InlineData("Medium")]
    [InlineData("Low")]
    public void TaskPriority_CreatesPriority_WhenValueIsAllowed(string priority)
    {
        // Arrange

        // Act
        var taskPriority = new TaskPriority(priority);

        // Assert
        Assert.Equal(priority, taskPriority.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Urgent")]
    [InlineData("InProgress")]
    public void TaskPriority_RejectsPriority_WhenValueIsNotAllowed(string priority)
    {
        // Arrange

        // Act
        Action createPriority = () => new TaskPriority(priority);

        // Assert
        Assert.Throws<ArgumentException>(createPriority);
    }

    [Fact]
    public void TaskEntity_CreatesTask_WhenPriorityIsProvided()
    {
        // Arrange
        var priority = new TaskPriority("High");

        // Act
        var task = TaskEntity.Create("Prepare interview notes", "idp-user-123", null, priority);

        // Assert
        Assert.Equal(priority, task.Priority);
    }
}
