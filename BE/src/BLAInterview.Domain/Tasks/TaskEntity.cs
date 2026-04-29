using BLAInterview.Domain.Common;
using BLAInterview.Domain.ValueObjects.Task;

namespace BLAInterview.Domain.Tasks;

public sealed class TaskEntity : BaseAuditableEntity
{
    public TaskDescription? Description { get; private set; }

    private TaskEntity(string title, string ownerId, TaskDescription? description)
    {
        Title = title;
        OwnerId = ownerId;
        Created = DateTimeOffset.UtcNow;
        CreatedBy = ownerId;
        Description = description;
    }
    public static TaskEntity Create(string title, string ownerId, TaskDescription? description) => new(title, ownerId, description);

    public string Title { get; }

    public string OwnerId { get; }
}
