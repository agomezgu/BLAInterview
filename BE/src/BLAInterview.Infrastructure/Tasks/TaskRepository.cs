using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;
using Npgsql;

namespace BLAInterview.Infrastructure.Tasks;

/// <summary>
/// Stores task entities in PostgreSQL using the configured Npgsql data source.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public TaskRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    /// <summary>
    /// Inserts a task row into the tasks table and returns the generated task identifier.
    /// </summary>
    public async Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken)
    {
        await using var command = _dataSource.CreateCommand(
            """
            INSERT INTO tasks (title, owner_id, created, created_by)
            VALUES ($1, $2, $3, $4)
            RETURNING id;
            """);

        command.Parameters.AddWithValue(task.Title);
        command.Parameters.AddWithValue(task.OwnerId);
        command.Parameters.AddWithValue(task.Created);
        command.Parameters.AddWithValue(task.CreatedBy ?? string.Empty);

        var taskId = (int)(await command.ExecuteScalarAsync(cancellationToken)
            ?? throw new InvalidOperationException("Task insert did not return an id."));
        task.Id = taskId;

        return taskId;
    }

    public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}