namespace BLAInterview.Domain.Tasks;

public sealed class Task
{
    private Task(string title, string ownerId)
    {
        Title = title;
        OwnerId = ownerId;
    }

    public string Title { get; }

    public string OwnerId { get; }

    public static Task Create(string title, string ownerId)
    {
        return new Task(title, ownerId);
    }
}
