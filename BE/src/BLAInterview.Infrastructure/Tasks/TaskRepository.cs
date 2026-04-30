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

    /// <summary>
    /// Reads task rows owned by the specified owner from the tasks table.
    /// </summary>
    public async Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken)
    {
        await using var command = _dataSource.CreateCommand(
            """
            SELECT id, title, owner_id, created, description, priority, status
            FROM tasks
            WHERE owner_id = $1;
            """);

        command.Parameters.AddWithValue(ownerId);

        var tasks = new List<TaskDto>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            tasks.Add(MapRowToTaskDto(reader));
        }

        return tasks;
    }

    /// <inheritdoc />
    public async Task<TaskDto?> GetOwnedTaskByIdAsync(
        int taskId,
        string ownerId,
        CancellationToken cancellationToken)
    {
        await using var command = _dataSource.CreateCommand(
            """
            SELECT id, title, owner_id, created, description, priority, status
            FROM tasks
            WHERE id = $1 AND owner_id = $2;
            """);

        command.Parameters.AddWithValue(taskId);
        command.Parameters.AddWithValue(ownerId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapRowToTaskDto(reader);
    }

    /// <inheritdoc />
    public async Task<TaskDto?> UpdateOwnedTaskAsync(
        int taskId,
        string ownerId,
        string? title,
        string? description,
        string? priority,
        string? status,
        CancellationToken cancellationToken)
    {
        await using var command = _dataSource.CreateCommand(
            """
            UPDATE tasks
            SET
                title = COALESCE($1, title),
                description = COALESCE($2, description),
                priority = COALESCE($3, priority),
                status = COALESCE($4, status)
            WHERE id = $5 AND owner_id = $6
            RETURNING id, title, owner_id, created, description, priority, status;
            """);

        command.Parameters.AddWithValue(title ?? (object)DBNull.Value);
        command.Parameters.AddWithValue(description ?? (object)DBNull.Value);
        command.Parameters.AddWithValue(priority ?? (object)DBNull.Value);
        command.Parameters.AddWithValue(status ?? (object)DBNull.Value);
        command.Parameters.AddWithValue(taskId);
        command.Parameters.AddWithValue(ownerId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapRowToTaskDto(reader);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken)
    {
        await using var command = _dataSource.CreateCommand(
            """
            DELETE FROM tasks
            WHERE id = $1 AND owner_id = $2;
            """);

        command.Parameters.AddWithValue(taskId);
        command.Parameters.AddWithValue(ownerId);

        var rows = await command.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    private static TaskDto MapRowToTaskDto(NpgsqlDataReader reader) =>
        new(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetFieldValue<DateTimeOffset>(3),
            reader.IsDBNull(4) ? null : reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.IsDBNull(6) ? null : reader.GetString(6));
}