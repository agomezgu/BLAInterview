using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;

namespace BLAInterview.Application.Abstractions;

public interface ITaskRepository
{
    Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken);
}
