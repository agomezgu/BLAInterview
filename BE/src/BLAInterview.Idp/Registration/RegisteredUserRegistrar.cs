using System.Net.Mail;
using BLAInterview.Idp.Data;
using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Registration;

/// <summary>
/// Result of an attempted user registration.
/// </summary>
public enum RegisterUserOutcome
{
    /// <summary>
    /// The request is invalid (missing fields or invalid email format).
    /// </summary>
    Invalid,

    /// <summary>
    /// The email address is already registered.
    /// </summary>
    DuplicateEmail,

    /// <summary>
    /// The user was created successfully.
    /// </summary>
    Created
}

/// <summary>
/// Registers local users in the IdP database, enforcing basic validation and uniqueness constraints.
/// </summary>
public sealed class RegisteredUserRegistrar(IdpDbContext context, PasswordHasher passwordHasher)
{
    /// <summary>
    /// Attempts to register a user and returns the outcome plus the created user id when successful.
    /// </summary>
    public async Task<(RegisterUserOutcome Outcome, int UserId)> RegisterAsync(
        RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password)
            || !IsEmailAddress(request.Email))
        {
            return (RegisterUserOutcome.Invalid, 0);
        }

        var normalizedEmail = request.Email.Trim().ToUpperInvariant();
        if (await context.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken))
        {
            return (RegisterUserOutcome.DuplicateEmail, 0);
        }

        var user = new RegisteredUser
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            PasswordHash = passwordHasher.Hash(request.Password),
            CreatedAt = DateTimeOffset.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return (RegisterUserOutcome.Created, user.Id);
    }

    private static bool IsEmailAddress(string email)
    {
        try
        {
            _ = new MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
