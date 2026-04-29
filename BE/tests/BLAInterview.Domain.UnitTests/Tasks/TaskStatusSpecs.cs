using BLAInterview.Domain.ValueObjects.Task;

namespace BLAInterview.Domain.UnitTests.Tasks;

public class TaskStatusSpecs
{
    [Theory]
    [InlineData("Pending")]
    [InlineData("InProgress")]
    [InlineData("Completed")]
    [InlineData("Cancelled")]
    public void TaskStatus_CreatesStatus_WhenValueIsAllowed(string status)
    {
        // Arrange

        // Act
        var taskStatus = new TaskStatus(status);

        // Assert
        Assert.Equal(status, taskStatus.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Blocked")]
    [InlineData("High")]
    public void TaskStatus_RejectsStatus_WhenValueIsNotAllowed(string status)
    {
        // Arrange

        // Act
        Action createStatus = () => new TaskStatus(status);

        // Assert
        Assert.Throws<ArgumentException>(createStatus);
    }
}
