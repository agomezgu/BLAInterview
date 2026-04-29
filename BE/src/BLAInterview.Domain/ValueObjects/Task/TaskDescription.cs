namespace BLAInterview.Domain.ValueObjects.Task;

public sealed record TaskDescription
{
    public string Value { get; }
    public TaskDescription(string value)
    {

         if (value.Length <= 10 || value.Length >= 50)
        {
            throw new ArgumentException("Task description must be more than 10 characters and less than 50 characters.", nameof(value));
        }
        Value = value;
    }

    
}
