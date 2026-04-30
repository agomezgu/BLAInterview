using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BLAInterview.Application.Tasks.Create;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BLAInterview.WebApi.UnitTests.Tasks;

public class TaskUpdateEndpointSpecs : IDisposable
{
    private const string AuthenticatedUserId = "idp-user-123";

    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public TaskUpdateEndpointSpecs()
    {
        this.factory = TaskEndpointTestFactory.Create();
        this.client = this.factory.CreateAuthenticatedClient(AuthenticatedUserId);
    }

    [Fact]
    public async Task TaskUpdateEndpoint_UpdatesEditableFields_WhenUserOwnsTask()
    {
        // Arrange
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();
        
        var update = new { title = "Updated title", status = "InProgress" };

        // Act
        var response = await this.client.PutAsJsonAsync($"/tasks/{taskId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task TaskUpdateEndpoint_ReturnsUpdatedDescriptionInBody_WhenDescriptionIsUpdated()
    {
        // Arrange: BE-API-006; validator requires 10 < length < 50.
        const string newDescription = "A task description in required length here.";
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes for description"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();
        var update = new { description = newDescription };

        // Act
        var response = await this.client.PutAsJsonAsync($"/tasks/{taskId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(newDescription, body.GetProperty("description").GetString());
    }

    [Fact]
    public async Task TaskUpdateEndpoint_ReturnsUpdatedPriorityInBody_WhenPriorityIsUpdated()
    {
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes for priority"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();
        const string newPriority = "High";
        var update = new { priority = newPriority };

        // Act
        var response = await this.client.PutAsJsonAsync($"/tasks/{taskId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(newPriority, body.GetProperty("priority").GetString());
    }

    [Fact]
    public async Task TaskUpdateEndpoint_ReturnsUpdatedStatusInBody_WhenStatusIsUpdated()
    {
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes for status"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();
        const string newStatus = "Completed";
        var update = new { status = newStatus };

        // Act
        var response = await this.client.PutAsJsonAsync($"/tasks/{taskId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(newStatus, body.GetProperty("status").GetString());
    }

    [Fact]
    public async Task TaskUpdateEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser()
    {
        // Arrange
        var otherUserClient = this.factory.CreateAuthenticatedClient("idp-user-456");
        var creationResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("My task only"));
        Assert.Equal(HttpStatusCode.Created, creationResponse.StatusCode);
        var taskId = (await creationResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("taskId").GetInt32();
        var update = new { title = "Hijack", status = "Pending" };

        try
        {
            // Act
            var response = await otherUserClient.PutAsJsonAsync($"/tasks/{taskId}", update);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            otherUserClient.Dispose();
        }
    }

    [Fact]
    public async Task TaskUpdateEndpoint_ReturnsValidationFailure_WhenStatusIsInvalid()
    {
        // Arrange
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();
        var update = new { title = "T", status = "NotAValidStatus" };

        // Act
        var response = await this.client.PutAsJsonAsync($"/tasks/{taskId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("TASK_STATUS_INVALID", errors[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task TaskUpdateEndpoint_ReturnsUpdatedTaskState_WhenUpdateSucceeds()
    {
        // Arrange
        var createResponse = await this.client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Original"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
        var taskId = createBody.GetProperty("taskId").GetInt32();
        const string newTitle = "New title after update";
        var update = new { title = newTitle, status = "Completed" };

        // Act
        var response = await this.client.PutAsJsonAsync($"/tasks/{taskId}", update);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<TaskDto>(cancellationToken: CancellationToken.None);
        Assert.NotNull(body);
        Assert.Equal(newTitle, body!.Title);
        Assert.Equal(AuthenticatedUserId, body!.OwnerId);
    }

    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
