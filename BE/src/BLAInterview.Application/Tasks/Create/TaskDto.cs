namespace BLAInterview.Application.Tasks.Create;

/// <summary>
/// API contract for creating a task.
/// </summary>
public sealed record CreateTaskDto(string Title);

/// <summary>
/// Task data returned by the application boundary.
/// </summary>
public sealed record TaskDto(
    int Id,
    string Title,
    string OwnerId,
    DateTimeOffset Created,
    string? Description = null,
    string? Priority = null,
    string? Status = null);
