using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Extensions;
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
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            request.Title,
            AuthenticatedUserId);
        var result = await createTaskHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.ToErrorDtos());
        }

        return Created($"/tasks/{result.Value}", new { taskId = result.Value, code = "TASK_CREATED" });
    }
}
