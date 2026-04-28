using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tasks")]
public sealed class TasksController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTasks(CancellationToken cancellationToken)
    {
        
        //var userId = User.FindFirst("sub")?.Value;

        return Ok(new TaskListResponse("Claim must Be Added", Array.Empty<object>()));
    }

    private sealed record TaskListResponse(string? UserId, object[] Tasks);
}
