using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;

namespace BLAInterview.Application.Abstractions;

public interface ITaskRepository
{
    /// <summary>
    /// Persists a new task entity and returns its generated identifier.
    /// </summary>
    Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all tasks owned by <paramref name="ownerId"/>.
    /// </summary>
    Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a single task if it exists and is owned by <paramref name="ownerId"/>; otherwise null.
    /// </summary>
    Task<TaskDto?> GetOwnedTaskByIdAsync(int taskId, string ownerId, CancellationToken cancellationToken);

    /// <summary>
    /// Persists update fields for a task that exists and is owned by the user, aligned with <see cref="TaskEntity"/> (title, description, priority, status).
    /// Returns null if the task does not exist or is not owned by <paramref name="ownerId"/>.
    /// </summary>
    Task<TaskDto?> UpdateOwnedTaskAsync(
        int taskId,
        string ownerId,
        string? title,
        string? description,
        string? priority,
        string? status,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a task that exists and is owned by <paramref name="ownerId"/>. Returns false if no row matched.
    /// </summary>
    Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken);
}
