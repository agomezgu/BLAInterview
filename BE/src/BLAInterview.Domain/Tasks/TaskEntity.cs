using BLAInterview.Domain.Common;

namespace BLAInterview.Domain.Tasks;

public sealed class TaskEntity : BaseAuditableEntity
{
    private TaskEntity(string title, string ownerId)
    {
        Title = title;
        OwnerId = ownerId;
        Created = DateTimeOffset.UtcNow;
        CreatedBy = ownerId;
    }

    public static TaskEntity Create(string title, string ownerId) => new(title, ownerId);

    public string Title { get; }

    public string OwnerId { get; }
}
