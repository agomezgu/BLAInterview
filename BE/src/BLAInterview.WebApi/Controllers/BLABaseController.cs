using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

public class BLABaseController : ControllerBase
{
    /// <summary>
    /// Authenticated user identifier from the JWT <c>sub</c> claim.
    /// </summary>
    protected string AuthenticatedUserId =>
        User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("Authenticated user id could not be inferred.");
}
