using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tasks")]
public sealed class TasksController (
    ICommandHandler<CreateTaskCommand, int> createTaskHandler) : BLABaseController
{
    [HttpGet]
    public IActionResult GetTasks(CancellationToken cancellationToken)
    {
        
        //var userId = User.FindFirst("sub")?.Value;

        return Ok(new { userId = "Claim must Be Added", tasks = Array.Empty<object>() });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TaskDto request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            request.Title,
            AuthenticatedUserId);
        var result = await createTaskHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.Select(error => error.Message));
        }

        return Created($"/tasks/{result.Value}", new { taskId = result.Value });
    }
}
