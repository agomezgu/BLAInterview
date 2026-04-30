# Task deletion endpoint success case
_Exported on 4/30/2026 at 01:39:51 GMT-5 from Cursor (3.2.16)_

---

**User**

/net-developer-tdd-green-writer 
TaskDeleteEndpoint_ReturnsSuccess_WhenUserDeletesOwnedTask
BE-API-007

---

**Cursor**

Delegating to the TDD green writer: reading the failing test, related code, and user story BE-API-007.



Implementing delete: adding `DeleteTaskCommand`, handler, repository method, DI registration, and controller action.



Implementing the delete flow: adding the application command and handler, extending the repository, and updating the controller and `Program.cs`.



Adding `DeleteOwnedTaskAsync` to all `ITaskRepository` test stubs.



Adding GET `/tasks/{id}` so a GET after delete returns 404. The `405` occurred because the route `tasks/{id}` matched the delete action but GET isn't allowed. Implementing a get-by-id path that returns 404 for missing or non-owned tasks.



Implementing `GetOwnedTaskByIdAsync` and `GetOwnedTaskQuery` to add `GET /tasks/{id}` and fix the 405 from route matching.



Adding `GetOwnedTaskByIdAsync` to all test stubs. Checking `TaskDto` and the WebApi `TaskListEndpointSpecs` for conflicts.



Here’s what was implemented for **BE-API-007** so `TaskDeleteEndpoint_ReturnsSuccess_WhenUserDeletesOwnedTask` (and the other `TaskDeleteEndpointSpecs`) pass.

### Delete path
- **`DeleteTaskCommand` + `DeleteTaskCommandHandler`** – deletes only when the task exists and `owner_id` matches; otherwise `TASK_NOT_FOUND`.
- **`ITaskRepository.DeleteOwnedTaskAsync`** – `DELETE FROM tasks WHERE id = $1 AND owner_id = $2`, returns whether a row was removed.
- **`TasksController`**: `DELETE /tasks/{id}` → **204 No Content** on success, **404** when not found (wrong user or id).

```103:120:c:\AndresG\AICourses\BLAInterview\BE\src\BLAInterview.WebApi\Controllers\TasksController.cs
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
```

- **`Program.cs`**: register `ICommandHandler<DeleteTaskCommand, bool>`.

### GET by id (needed for the suite)
`TaskDeleteEndpoint_GetTaskReturnsNotFound_AfterSuccessfulDelete` was getting **405** because only `DELETE` was bound to `tasks/{id}`. To align with the story and fix that, **`GetOwnedTaskQuery`**, **`GetOwnedTaskByIdAsync`**, and **`GET /tasks/{id}`** were added (404 when the task is missing or not owned—including after delete).

### Tests / stubs
- **`TaskEndpointTestFactory.StubTaskRepository`**: in-memory `GetOwnedTaskByIdAsync` and `DeleteOwnedTaskAsync`.
- Other `ITaskRepository` fakes in Application tests were updated with the new members.

`dotnet test` for **WebApi.UnitTests** (26 tests) and **Application.UnitTests** (14 run, 3 still skipped in `DeleteTaskCommandHandlerSpecs`) both succeed.

If you want the **Application** delete handler specs unskipped and filled in next, say so and we can wire those tests to match the new command/handler.

