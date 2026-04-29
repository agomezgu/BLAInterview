using System.Collections.Concurrent;
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
    private static readonly ConcurrentBag<TaskDto> CreatedTasks = [];

    [HttpGet]
    public IActionResult GetTasks(CancellationToken cancellationToken)
    {
        var ownedTasks = CreatedTasks
            .Where(task => task.OwnerId == AuthenticatedUserId)
            .ToList();

        return Ok(ownedTasks);
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

        CreatedTasks.Add(new TaskDto(result.Value, request.Title, AuthenticatedUserId, DateTimeOffset.UtcNow));

        return Created($"/tasks/{result.Value}", new { taskId = result.Value, code = "TASK_CREATED" });
    }
}
