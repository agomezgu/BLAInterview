using System.Net;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.Idp.Controllers;

[AllowAnonymous]
[Route("home")]
public sealed class HomeController(IIdentityServerInteractionService interaction) : Controller
{
    [HttpGet("error")]
    public async Task<IActionResult> Error(string? errorId)
    {
        if (string.IsNullOrEmpty(errorId))
        {
            return Content(
                "<html><body><p>No error details (missing errorId).</p></body></html>",
                "text/html; charset=utf-8");
        }

        var message = await interaction.GetErrorContextAsync(errorId);
        if (message is null)
        {
            return Content(
                "<html><body><p>Unknown error reference.</p></body></html>",
                "text/html; charset=utf-8");
        }

        var error = WebUtility.HtmlEncode(message.Error ?? "");
        var description = WebUtility.HtmlEncode(message.ErrorDescription ?? "");
        var html =
            $"""
            <!DOCTYPE html>
            <html lang="en">
            <head><meta charset="utf-8"/><title>Sign-in error</title></head>
            <body>
            <h1>Sign-in error</h1>
            <p><strong>{error}</strong></p>
            <pre style="white-space:pre-wrap">{description}</pre>
            <p><a href="/Account/Login">Back to login</a></p>
            </body>
            </html>
            """;
        return Content(html, "text/html; charset=utf-8");
    }
}
