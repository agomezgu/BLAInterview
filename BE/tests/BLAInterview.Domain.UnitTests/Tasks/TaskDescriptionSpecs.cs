namespace BLAInterview.Domain.UnitTests.Tasks;

public class TaskDescriptionSpecs
{
    [Fact]
    public void TaskDescription_CreatesDescription_WhenLengthIsBetweenMinimumAndMaximum()
    {
        // Arrange
        var description = "Eleven ok.";

        // Act
        var taskDescription = BLAInterview.Domain.Tasks.TaskDescription.Create(description);

        // Assert
        Assert.Equal(description, taskDescription.Value);
    }

    [Fact]
    public void TaskDescription_RejectsDescription_WhenLengthIsTenCharactersOrLess()
    {
        // Arrange
        var descriptions = new[]
        {
            "",
            "Short",
            "1234567890"
        };

        // Act
        var createDescriptions = descriptions
            .Select<Action>(description => () => BLAInterview.Domain.Tasks.TaskDescription.Create(description))
            .ToArray();

        // Assert
        foreach (var createDescription in createDescriptions)
        {
            Assert.Throws<ArgumentException>(createDescription);
        }
    }

    [Fact]
    public void TaskDescription_RejectsDescription_WhenLengthIsFiftyCharactersOrMore()
    {
        // Arrange
        var descriptions = new[]
        {
            new string('A', 50),
            new string('A', 51)
        };

        // Act
        var createDescriptions = descriptions
            .Select<Action>(description => () => BLAInterview.Domain.Tasks.TaskDescription.Create(description))
            .ToArray();

        // Assert
        foreach (var createDescription in createDescriptions)
        {
            Assert.Throws<ArgumentException>(createDescription);
        }
    }
}
