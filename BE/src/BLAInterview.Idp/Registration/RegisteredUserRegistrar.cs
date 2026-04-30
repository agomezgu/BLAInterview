using System.Net.Mail;
using BLAInterview.Idp.Data;
using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Registration;

public enum RegisterUserOutcome
{
    Invalid,
    DuplicateEmail,
    Created
}

public sealed class RegisteredUserRegistrar(IdpDbContext context, PasswordHasher passwordHasher)
{
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
