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

    /// <summary>
    /// Transitions the task to <paramref name="newStatus"/> when the lifecycle allows it; otherwise throws.
    /// </summary>
    public TaskEntity TransitionTo(TaskStatus newStatus)
    {
        if (newStatus is null)
        {
            throw new ArgumentNullException(nameof(newStatus));
        }

        if (!IsTransitionAllowed(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Status cannot change from {Status?.Value ?? "(none)"} to {newStatus.Value}.");
        }

        Status = newStatus;
        return this;
    }

    /// <summary>
    /// Applies status from an update payload (e.g. raw string). Invalid labels throw
    /// <see cref="ArgumentException"/> from <see cref="TaskStatus"/>, so the current status is not changed.
    /// Allowed values then go through the same transition rules as <see cref="TransitionTo"/>.
    /// </summary>
    public TaskEntity ApplyStatusUpdate(string rawStatus)
    {
        var newStatus = new TaskStatus(rawStatus);
        return TransitionTo(newStatus);
    }

    /// <summary>
    /// Returns whether a transition from the current status label to <paramref name="toStatus"/> is allowed
    /// (same rules as <see cref="TransitionTo"/> / <see cref="ApplyStatusUpdate"/>).
    /// </summary>
    public static bool IsStatusTransitionAllowed(string? fromStatus, string toStatus)
    {
        var to = new TaskStatus(toStatus);
        TaskStatus? from = fromStatus is null ? null : new TaskStatus(fromStatus);
        return IsTransitionAllowed(from, to);
    }

    private static bool IsTransitionAllowed(TaskStatus? from, TaskStatus to)
    {
        if (from is null)
        {
            return true;
        }

        if (string.Equals(from.Value, to.Value, StringComparison.Ordinal))
        {
            return true;
        }

        return (from.Value, to.Value) switch
        {
            ("Pending", "InProgress") => true,
            ("Pending", "Cancelled") => true,
            ("InProgress", "Completed") => true,
            ("InProgress", "Cancelled") => true,
            _ => false
        };
    }

    public string Title { get; }

    public string OwnerId { get; }
}
