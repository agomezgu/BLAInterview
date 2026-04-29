using BLAInterview.Domain.Common;
using BLAInterview.Domain.ValueObjects.Task;
using TaskStatus = BLAInterview.Domain.ValueObjects.Task.TaskStatus;

namespace BLAInterview.Domain.Tasks;

/// <summary>
/// Represents a task owned by a user in the domain model.
/// </summary>
public sealed class TaskEntity : BaseAuditableEntity
{
    public TaskDescription? Description { get; private set; }

    public TaskPriority? Priority { get; private set; }

    public TaskStatus? Status { get; private set; }

    private TaskEntity(string title, string ownerId, TaskDescription? description, TaskPriority? priority, TaskStatus? status)
    {
        Title = title;
        OwnerId = ownerId;
        Created = DateTimeOffset.UtcNow;
        CreatedBy = ownerId;
        Description = description;
        Priority = priority;
        Status = status;
    }

    /// <summary>
    /// Creates a task with optional description, priority, and status value objects.
    /// </summary>
    public static TaskEntity Create(
        string title,
        string ownerId,
        TaskDescription? description = null,
        TaskPriority? priority = null,
        TaskStatus? status = null) => new(title, ownerId, description, priority, status);

    public string Title { get; }

    public string OwnerId { get; }
}
