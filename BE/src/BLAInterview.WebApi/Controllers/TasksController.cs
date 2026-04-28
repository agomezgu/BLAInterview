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

    [HttpPost]
    public IActionResult CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        return Created("/tasks", new TaskCreationResponse(request.Title));
    }

    private sealed record TaskListResponse(string? UserId, object[] Tasks);

    public sealed record CreateTaskRequest(string Title);

    public sealed record TaskCreationResponse(string Title);
}
