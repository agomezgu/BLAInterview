using System.Linq;
using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Extensions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.Delete;
using BLAInterview.Application.Tasks.List;
using BLAInterview.Application.Tasks.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tasks")]
public sealed class TasksController (
    ICommandHandler<CreateTaskCommand, int> createTaskHandler,
    ICommandHandler<ListOwnedTasksQuery, IReadOnlyCollection<TaskDto>> listOwnedTasksHandler,
    ICommandHandler<GetOwnedTaskQuery, TaskDto> getOwnedTaskHandler,
    ICommandHandler<UpdateTaskCommand, TaskDto> updateTaskHandler,
    ICommandHandler<DeleteTaskCommand, bool> deleteTaskHandler) : BLABaseController
{
   
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTaskById(int id, CancellationToken cancellationToken)
    {
        var query = new GetOwnedTaskQuery(id, AuthenticatedUserId);
        var result = await getOwnedTaskHandler.HandleAsync(query, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Metadata.GetValueOrDefault("Code") is "TASK_NOT_FOUND"))
            {
                return NotFound();
            }

            return BadRequest(result.ToErrorDtos());
        }

        return Ok(result.Value);
    }

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

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTask(
        int id,
        [FromBody] UpdateTaskDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTaskCommand(
            id,
            AuthenticatedUserId,
            request.Title,
            request.Description,
            request.Priority,
            request.Status);
        var result = await updateTaskHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Metadata.GetValueOrDefault("Code") is "TASK_NOT_FOUND"))
            {
                return NotFound();
            }

            return BadRequest(result.ToErrorDtos());
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id, CancellationToken cancellationToken)
    {
        var command = new DeleteTaskCommand(id, AuthenticatedUserId);
        var result = await deleteTaskHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailed)
        {
            if (result.Errors.Any(e => e.Metadata.GetValueOrDefault("Code") is "TASK_NOT_FOUND"))
            {
                return NotFound();
            }

            return BadRequest(result.ToErrorDtos());
        }

        return NoContent();
    }
}
