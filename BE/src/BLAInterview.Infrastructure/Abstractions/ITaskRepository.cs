using BLAInterview.Domain.Tasks;

namespace BLAInterview.Infrastructure.Abstractions;

public interface ITaskRepository
{
    Task<Guid> CreateAsync(TaskEntity task);
    
}