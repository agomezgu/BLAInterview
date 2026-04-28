using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BLAInterview.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("tasks")]
public sealed class TasksController : ControllerBase
{
    [HttpGet]
    public IActionResult GetTasks()
    {
        return Ok(Array.Empty<object>());
    }
}
