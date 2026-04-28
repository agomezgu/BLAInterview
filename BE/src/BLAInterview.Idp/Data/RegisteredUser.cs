namespace BLAInterview.Idp.Data;

public sealed class RegisteredUser
{
    public Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Email { get; init; }

    public required string PasswordHash { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}
