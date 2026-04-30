namespace BLAInterview.Domain.ValueObjects.Task;

/// <summary>
/// Represents a task description with a constrained length in the domain.
/// </summary>
public sealed record TaskDescription
{
    public string Value { get; }

    /// <summary>
    /// Creates a task description after verifying it is within the supported length range.
    /// </summary>
    /// <param name="value">Description text.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not within the supported length range.</exception>
    public TaskDescription(string value)
    {
        if (value.Length <= 10 || value.Length >= 50)
        {
            throw new ArgumentException("Task description must be more than 10 characters and less than 50 characters.", nameof(value));
        }
        Value = value;
    }
}
