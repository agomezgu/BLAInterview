using BLAInterview.Domain.Tasks;
using BLAInterview.Infrastructure.Abstractions;
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
    /// Inserts a task row into the tasks table and returns the task identifier.
    /// </summary>
    public async Task<Guid> AddAsync(TaskEntity task)
    {
        await using var command = _dataSource.CreateCommand(
            """
            INSERT INTO tasks (id, title, owner_id, created, created_by)
            VALUES ($1, $2, $3, $4, $5);
            """);

        command.Parameters.AddWithValue(task.Id);
        command.Parameters.AddWithValue(task.Title);
        command.Parameters.AddWithValue(task.OwnerId);
        command.Parameters.AddWithValue(task.Created);
        command.Parameters.AddWithValue(task.CreatedBy ?? string.Empty);

        await command.ExecuteNonQueryAsync();

        return task.Id;
    }
}