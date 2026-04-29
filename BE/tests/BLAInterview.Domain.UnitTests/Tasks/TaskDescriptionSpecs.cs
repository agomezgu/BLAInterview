using BLAInterview.Domain.ValueObjects.Task;

namespace BLAInterview.Domain.UnitTests.Tasks;

public class TaskDescriptionSpecs
{
    public static TheoryData<string> DescriptionsWithFiftyCharactersOrMore => new()
    {
        new string('A', 50),
        new string('A', 51)
    };

    [Fact]
    public void TaskDescription_CreatesDescription_WhenLengthIsBetweenMinimumAndMaximum()
    {
        // Arrange
        var description = "Eleven okay.";

        // Act
        var taskDescription = new TaskDescription(description);

        // Assert
        Assert.Equal(description, taskDescription.Value);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("Short")]
    [InlineData("1234567890")]
    public void TaskDescription_RejectsDescription_WhenLengthIsTenCharactersOrLess(string description)
    {
        // Act
        var createDescription = () => new TaskDescription(description);

        // Assert
        Assert.Throws<ArgumentException>(createDescription);
    }

    [Theory]
    [MemberData(nameof(DescriptionsWithFiftyCharactersOrMore))]
    public void TaskDescription_RejectsDescription_WhenLengthIsFiftyCharactersOrMore(string description)
    {
        // Act
        var createDescription = () => new TaskDescription(description);

        // Assert
        Assert.Throws<ArgumentException>(createDescription);
    }
}
