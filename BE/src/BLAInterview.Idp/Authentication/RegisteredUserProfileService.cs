using BLAInterview.Idp.Data;
using Duende.IdentityModel;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Authentication;

public sealed class RegisteredUserProfileService(IdpDbContext db) : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subject = context.Subject?.GetSubjectId();
        if (subject is null || !int.TryParse(subject, out var userId))
        {
            return;
        }

        var user = await db.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == userId);
        if (user is null)
        {
            return;
        }

        var claims = new List<System.Security.Claims.Claim>
        {
            new(JwtClaimTypes.Name, user.Name),
            new(JwtClaimTypes.Email, user.Email),
            new(JwtClaimTypes.PreferredUserName, user.Email)
        };

        context.IssuedClaims = claims
            .Where(c => context.RequestedClaimTypes.Contains(c.Type))
            .ToList();
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subject = context.Subject?.GetSubjectId();
        if (subject is null || !int.TryParse(subject, out var userId))
        {
            context.IsActive = false;
            return;
        }

        context.IsActive = await db.Users.AnyAsync(u => u.Id == userId);
    }
}
