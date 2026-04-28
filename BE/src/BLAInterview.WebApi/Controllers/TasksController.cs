using BLAInterview.WebApi.Application.Abstractions;
using BLAInterview.WebApi.Application.Tasks;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tasks")]
public sealed class TasksController(
    ICommandHandler<CreateTaskCommand, CreateTaskResult> createTaskHandler) : ControllerBase
{
    [HttpGet]
    public IActionResult GetTasks(CancellationToken cancellationToken)
    {
        
        //var userId = User.FindFirst("sub")?.Value;

        return Ok(new TaskListResponse("Claim must Be Added", Array.Empty<object>()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTaskCommand(
            request.Title,
            User.FindFirst("sub")?.Value ?? string.Empty);
        var result = await createTaskHandler.HandleAsync(command, cancellationToken);

        if (result.IsFailed)
        {
            return BadRequest(new TaskCreationFailureResponse(
                result.Errors.Select(error => new TaskCreationErrorResponse(
                    GetErrorCode(error),
                    error.Message)).ToArray()));
        }

        var task = result.Value;

        return Created($"/tasks/{task.Id}", new TaskCreationResponse(task.Id, task.Title, task.OwnerId));
    }

    private sealed record TaskListResponse(string? UserId, object[] Tasks);

    public sealed record CreateTaskRequest(string Title);

    public sealed record TaskCreationResponse(Guid Id, string Title, string OwnerId);

    private sealed record TaskCreationFailureResponse(TaskCreationErrorResponse[] Errors);

    private sealed record TaskCreationErrorResponse(string Code, string Message);

    private static string GetErrorCode(IError error)
    {
        return error.Metadata.TryGetValue("Code", out var code)
            ? code?.ToString() ?? "TASK_CREATION_FAILED"
            : "TASK_CREATION_FAILED";
    }
}
