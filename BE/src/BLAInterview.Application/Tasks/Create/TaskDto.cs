namespace BLAInterview.Application.Tasks.Create;

public sealed record CreateTaskDto(string Title);

public sealed record TaskDto(
    int Id,
    string Title,
    string OwnerId,
    DateTimeOffset Created,
    string? Description = null,
    string? Priority = null,
    string? Status = null);
