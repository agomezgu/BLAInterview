namespace BLAInterview.Application.Tasks.Update;

/// <summary>
/// API contract for task updates, aligned with <see cref="BLAInterview.Domain.Tasks.TaskEntity"/> (title, description, priority, status).
/// Owner and identifier are not in the body; owner comes from the authenticated user and the task id from the route.
/// Null for a property means "leave that field unchanged".
/// </summary>
public sealed record UpdateTaskDto(
    string? Title,
    string? Description,
    string? Priority,
    string? Status);
