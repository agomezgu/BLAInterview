using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.Abstractions;
using Npgsql;

namespace BLAInterview.Infrastructure.Tasks;


public class TaskRepository : ITaskRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public TaskRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Guid> CreateAsync(TaskEntity task)
    {
        throw new NotImplementedException();
    }
}

 