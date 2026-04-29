using BLAInterview.Domain.Common;
using BLAInterview.Domain.ValueObjects.Task;

namespace BLAInterview.Domain.Tasks;

/// <summary>
/// Represents a task owned by a user in the domain model.
/// </summary>
public sealed class TaskEntity : BaseAuditableEntity
{
    public TaskDescription? Description { get; private set; }

    public TaskPriority? Priority { get; private set; }

    private TaskEntity(string title, string ownerId, TaskDescription? description, TaskPriority? priority)
    {
        Title = title;
        OwnerId = ownerId;
        Created = DateTimeOffset.UtcNow;
        CreatedBy = ownerId;
        Description = description;
        Priority = priority;
    }

    /// <summary>
    /// Creates a task with optional description and priority value objects.
    /// </summary>
    public static TaskEntity Create(string title, string ownerId, TaskDescription? description, TaskPriority? priority) => new(title, ownerId, description, priority);

    public string Title { get; }

    public string OwnerId { get; }
}
