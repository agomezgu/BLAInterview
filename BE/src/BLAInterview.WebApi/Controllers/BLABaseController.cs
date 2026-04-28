using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

public class BLABaseController : ControllerBase
{
    protected string AuthenticatedUserId =>
        User.FindFirst("sub")?.Value
        ?? throw new UnauthorizedAccessException("Authenticated user id could not be inferred.");
}
