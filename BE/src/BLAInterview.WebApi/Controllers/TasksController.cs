using System.Collections.Concurrent;
using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Extensions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.List;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tasks")]
public sealed class TasksController (
    ICommandHandler<CreateTaskCommand, int> createTaskHandler,
    ICommandHandler<ListOwnedTasksQuery, IReadOnlyCollection<TaskDto>> listOwnedTasksHandler) : BLABaseController
{
   
    [HttpGet]
    public async Task<IActionResult> GetTasks(CancellationToken cancellationToken)
    {
        var command = new ListOwnedTasksQuery(AuthenticatedUserId);
        var result = await listOwnedTasksHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(result.ToErrorDtos());
        }

        return Ok(result.Value);
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
