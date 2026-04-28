using BLAInterview.Domain.Common;

namespace BLAInterview.Domain.Tasks;

public sealed class Task : BaseAuditableEntity
{
    private Task(string title, string ownerId)
    {
        Id = Guid.NewGuid();
        Title = title;
        OwnerId = ownerId;
        Created = DateTimeOffset.UtcNow;
        CreatedBy = ownerId;
    }

    public static Task Create(string title, string ownerId) => new(title, ownerId);

    public string Title { get; }

    public string OwnerId { get; }
}
