using BLAInterview.Idp.Data;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Authentication;

public sealed class RegisteredUserPasswordValidator(
    IdpDbContext context,
    PasswordHasher passwordHasher) : IResourceOwnerPasswordValidator
{
    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(validationContext.UserName)
            || string.IsNullOrWhiteSpace(validationContext.Password))
        {
            validationContext.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Invalid user credentials.");
            return;
        }

        var email = validationContext.UserName.Trim().ToUpperInvariant();
        var user = await context.Users.SingleOrDefaultAsync(
            registeredUser => registeredUser.NormalizedEmail == email);

        if (user is null || !passwordHasher.Verify(validationContext.Password, user.PasswordHash))
        {
            validationContext.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "Invalid user credentials.");
            return;
        }

        validationContext.Result = new GrantValidationResult(
            subject: user.Id.ToString(),
            authenticationMethod: "password");
    }
}
