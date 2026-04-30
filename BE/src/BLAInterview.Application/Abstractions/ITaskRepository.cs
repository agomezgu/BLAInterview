using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;

namespace BLAInterview.Application.Abstractions;

public interface ITaskRepository
{
    Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken);

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
}
