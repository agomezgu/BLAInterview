using BLAInterview.Idp.Data;
using BLAInterview.Idp.Models;
using BLAInterview.Idp.Registration;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BLAInterview.Idp.Controllers;

[AllowAnonymous]
[Route("Account")]
public sealed class AccountController(
    IIdentityServerInteractionService interaction,
    IdpDbContext db,
    PasswordHasher passwordHasher,
    RegisteredUserRegistrar registrar) : Controller
{
    [HttpGet("Login")]
    public IActionResult Login(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        if (!interaction.IsValidReturnUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var returnUrl = model.ReturnUrl ?? Url.Content("~/");
        if (!interaction.IsValidReturnUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        var normalizedEmail = model.Email.Trim().ToUpperInvariant();
        var user = await db.Users.SingleOrDefaultAsync(
            u => u.NormalizedEmail == normalizedEmail,
            cancellationToken);

        if (user is null || !passwordHasher.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        var identityUser = new IdentityServerUser(user.Id.ToString())
        {
            DisplayName = user.Name,
            IdentityProvider = IdentityServerConstants.LocalIdentityProvider,
            AuthenticationTime = DateTime.UtcNow,
            AdditionalClaims =
            [
                new System.Security.Claims.Claim(JwtClaimTypes.Name, user.Name),
                new System.Security.Claims.Claim(JwtClaimTypes.Email, user.Email),
                new System.Security.Claims.Claim(JwtClaimTypes.PreferredUserName, user.Email)
            ]
        };

        await HttpContext.SignInAsync(identityUser);
        return Redirect(returnUrl);
    }

    [HttpGet("Register")]
    public IActionResult Register(string? returnUrl)
    {
        returnUrl ??= Url.Content("~/");
        if (!interaction.IsValidReturnUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost("Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        var returnUrl = model.ReturnUrl ?? Url.Content("~/");
        if (!interaction.IsValidReturnUrl(returnUrl))
        {
            returnUrl = Url.Content("~/");
        }

        model.ReturnUrl = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var (outcome, _) = await registrar.RegisterAsync(
            new RegisterUserRequest(model.Name, model.Email, model.Password),
            cancellationToken);

        if (outcome == RegisterUserOutcome.DuplicateEmail)
        {
            ModelState.AddModelError(string.Empty, "An account with this email already exists.");
            return View(model);
        }

        if (outcome == RegisterUserOutcome.Invalid)
        {
            ModelState.AddModelError(string.Empty, "Registration could not be completed. Check your details.");
            return View(model);
        }

        TempData["RegistrationSuccess"] = "Account created. Please sign in.";
        return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl });
    }

    [HttpGet("Logout")]
    public async Task<IActionResult> Logout(string? logoutId)
    {
        var logout = await interaction.GetLogoutContextAsync(logoutId);
        if (User?.Identity?.IsAuthenticated == true)
        {
            await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);
        }

        if (logout?.PostLogoutRedirectUri is { } uri)
        {
            return Redirect(uri);
        }

        return Redirect(Url.Content("~/"));
    }
}
