namespace BLAInterview.Domain.ValueObjects.Task;

/// <summary>
/// Represents the allowed lifecycle status values a task can have.
/// </summary>
public sealed record TaskStatus
{
    private static readonly HashSet<string> AllowedValues = ["Pending", "InProgress", "Completed", "Cancelled"];

    public string Value { get; }

    /// <summary>
    /// Creates a task status after verifying it is one of the supported domain values.
    /// </summary>
    /// <param name="value">Status label, expected to be Pending, InProgress, Completed, or Cancelled.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is not a supported status.</exception>
    public TaskStatus(string value)
    {
        if (!AllowedValues.Contains(value))
        {
            throw new ArgumentException("Task status must be one of Pending, InProgress, Completed, or Cancelled.", nameof(value));
        }

        Value = value;
    }
}
