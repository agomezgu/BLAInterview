using BLAInterview.Domain.Tasks;

namespace BLAInterview.Infrastructure.Abstractions;

/// <summary>
/// Defines persistence operations for task entities within the infrastructure boundary.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Persists the supplied task and returns the identifier assigned by the domain entity.
    /// </summary>
    Task<Guid> AddAsync(TaskEntity task);
}