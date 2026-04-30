# Task delete endpoint success criteria
_Exported on 4/30/2026 at 01:39:04 GMT-5 from Cursor (3.2.16)_

---

**User**

/net-developer-tdd-red-writer 
TaskDeleteEndpoint_ReturnsSuccess_WhenUserDeletesOwnedTask
BE-API-007

---

**Cursor**

Gathering BE-API-007 requirements and existing delete-endpoint test patterns, then delegating the red-writer subagent to implement the failing test.



Delegating to the net-developer-tdd-red-writer with full context: story BE-API-007, file path, `TaskUpdateEndpointSpecs` pattern, `TaskEndpointTestFactory` + `StubTaskRepository`, and expected behavior (POST then DELETE, assert success—204 is typical).



Verifying the implementation on disk and running the test.



Red-phase test is in place and fails for the right reason. Updating the class doc comment that still says tests are skipped.



## Result

The **net-developer-tdd-red-writer** subagent updated **`TaskDeleteEndpoint_ReturnsSuccess_WhenUserDeletesOwnedTask`** for **BE-API-007** (delete owned task).

### What the spec does

- Uses the same setup as `TaskUpdateEndpointSpecs`: `TaskEndpointTestFactory`, `CreateAuthenticatedClient("idp-user-123")`, `IDisposable`.
- **Arrange:** `POST /tasks` with `CreateTaskDto`, read `taskId` from JSON.
- **Act:** `DELETE /tasks/{taskId}`.
- **Assert:** `HttpStatusCode.NoContent` (204).
- `Skip` was removed from this test only; the other three specs are still skipped.

```26:43:c:\AndresG\AICourses\BLAInterview\BE\tests\BLAInterview.WebApi.UnitTests\Tasks\TaskDeleteEndpointSpecs.cs
    [Fact]
    public async Task TaskDeleteEndpoint_ReturnsSuccess_WhenUserDeletesOwnedTask()
    {
        // Arrange
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes for delete"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();

        // Act
        var response = await this.client.DeleteAsync($"/tasks/{taskId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
```

### Red state (verified with `dotnet test`)

- **Expected:** `NoContent` (204)  
- **Actual:** `MethodNotAllowed` (405) — there is no `DELETE` route for `/tasks/{id}` yet, which is the intended TDD failure before the green implementation.

The class XML summary was adjusted so it no longer says everything is skipped.

---

**User**

/net-developer-tdd-red-writer 
TaskDeleteEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser
BE-API-007

---

**Cursor**

Reviewing the spec file and the "another user" pattern, then delegating the red-writer to implement the failing test.



## `TaskDeleteEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser` (BE-API-007)

The **net-developer-tdd-red-writer** subagent replaced the skipped placeholder with a full spec that mirrors `TaskUpdateEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser`.

### Behavior

- Owner `idp-user-123` creates a task with `POST /tasks` and `CreateTaskDto("My task only")`, then reads `taskId`.
- `idp-user-456` calls `DELETE /tasks/{taskId}` on a second client, with `try` / `finally` and `Dispose` on the other client.

```45:69:c:\AndresG\AICourses\BLAInterview\BE\tests\BLAInterview.WebApi.UnitTests\Tasks\TaskDeleteEndpointSpecs.cs
    [Fact]
    public async Task TaskDeleteEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser()
    {
        // Arrange
        var otherUserClient = this.factory.CreateAuthenticatedClient("idp-user-456");
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("My task only"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();

        try
        {
            // Act
            var response = await otherUserClient.DeleteAsync($"/tasks/{taskId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            otherUserClient.Dispose();
        }
    }
```

### Red (verified with `dotnet test`)

- **Expected:** `NotFound` (404)  
- **Actual:** `MethodNotAllowed` (405) — no `DELETE` for `/tasks/{id}` yet, so this fails until delete is implemented and the “wrong owner” branch returns 404.

---

**User**

/net-developer-tdd-red-writer 
TaskDeleteEndpoint_ReturnsNotFound_WhenTaskIdDoesNotExist
BE-API-007

---

**Cursor**

Delegating the red-writer to implement the missing-ID delete spec (assert 404; use a literal id unlikely to exist, e.g. 99999).



## `TaskDeleteEndpoint_ReturnsNotFound_WhenTaskIdDoesNotExist` (BE-API-007)

The **net-developer-tdd-red-writer** subagent removed `Skip` and added a focused spec: authenticated client deletes a **non-existent** id and expects **404**.

```71:82:c:\AndresG\AICourses\BLAInterview\BE\tests\BLAInterview.WebApi.UnitTests\Tasks\TaskDeleteEndpointSpecs.cs
    [Fact]
    public async Task TaskDeleteEndpoint_ReturnsNotFound_WhenTaskIdDoesNotExist()
    {
        // Arrange
        const int taskId = 999_999;

        // Act
        var response = await this.client.DeleteAsync($"/tasks/{taskId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
```

### Red (`dotnet test`)

- **Expected:** `NotFound` (404)  
- **Actual:** `MethodNotAllowed` (405) — no `DELETE /tasks/{id}` yet, consistent with the other delete endpoint specs.

---

**User**

/net-developer-tdd-red-writer 
TaskDeleteEndpoint_GetTaskReturnsNotFound_AfterSuccessfulDelete
BE-API-007

---

**Cursor**

Delegating implementation: the story requires creating a task, deleting it, then GET `/tasks/{id}` expecting 404. `CreateTask` returns `Location: /tasks/{id}` — the follow-up should use that shape.



All delete endpoint specs are active. Updating the class summary comment.



## `TaskDeleteEndpoint_GetTaskReturnsNotFound_AfterSuccessfulDelete` (BE-API-007)

The **net-developer-tdd-red-writer** subagent removed `Skip` and added an end-to-end check: create → delete → `GET` the same id and expect **404** (aligned with *“requests the deleted task again”*).

```84:102:c:\AndresG\AICourses\BLAInterview\BE\tests\BLAInterview.WebApi.UnitTests\Tasks\TaskDeleteEndpointSpecs.cs
    [Fact]
    public async Task TaskDeleteEndpoint_GetTaskReturnsNotFound_AfterSuccessfulDelete()
    {
        // Arrange
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Task to delete then fetch"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();

        // Act
        var deleteResponse = await this.client.DeleteAsync($"/tasks/{taskId}");
        var getResponse = await this.client.GetAsync($"/tasks/{taskId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
```

### Red (`dotnet test`)

- Fails on the **first** assert: **expected** `NoContent` (204), **actual** `MethodNotAllowed` (405) — `DELETE` is not implemented yet, so the scenario stops there (same as the other delete specs).

The class doc comment was updated: every scenario in this file is now a real `[Fact]`, not skipped.

**Green follow-up note:** The app today only has `GET /tasks` (list), not `GET /tasks/{id}`. After `DELETE` exists, you will still need a **get-by-id** (or equivalent) behavior that returns **404** for a removed task so the second assertion can pass.

