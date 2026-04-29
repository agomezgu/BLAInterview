namespace BLAInterview.Domain.ValueObjects.Task;

/// <summary>
/// Represents the allowed priority levels a task can be assigned.
/// </summary>
public sealed record TaskPriority
{
    private static readonly HashSet<string> AllowedValues = ["High", "Medium", "Low"];

    public string Value { get; }

    /// <summary>
    /// Creates a task priority after verifying it is one of the supported domain values.
    /// </summary>
    /// <param name="value">Priority label, expected to be High, Medium, or Low.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not a supported priority.</exception>
    public TaskPriority(string value)
    {
        if (!AllowedValues.Contains(value))
        {
            throw new ArgumentException("Task priority must be one of High, Medium, or Low.", nameof(value));
        }

        Value = value;
    }
}
