
using BLAInterview.Domain.Tasks;
using TaskStatus = BLAInterview.Domain.ValueObjects.Task.TaskStatus;

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
        var taskStatus = new TaskStatus(status);

        // Act
        var task = TaskEntity.Create("Prepare interview notes", "idp-user-123", status: taskStatus);

        // Assert
        Assert.Equal(status, task.Status?.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Blocked")]
    [InlineData("High")]
    public void TaskStatus_RejectsStatus_WhenValueIsNotAllowed(string status)
    {
        // Arrange

        // Act
        Action createStatus = () =>
        {
            var taskStatus = new TaskStatus(status);
            TaskEntity.Create("Prepare interview notes", "idp-user-123", status: taskStatus);
        };

        // Assert
        Assert.Throws<ArgumentException>(createStatus);
    }
}
