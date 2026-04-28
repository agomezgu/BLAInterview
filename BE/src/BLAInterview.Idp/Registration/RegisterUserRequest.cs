namespace BLAInterview.Idp.Registration;

public sealed record RegisterUserRequest(
    string Name,
    string Email,
    string Password);
