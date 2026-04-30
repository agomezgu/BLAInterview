using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BLAInterview.Application.Tasks.Create;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BLAInterview.WebApi.UnitTests.Tasks;

/// <summary>
/// BE-API-007: HTTP delete for an owned task. Uses <c>TaskEndpointTestFactory</c> and an
/// authenticated client, same as <c>TaskUpdateEndpointSpecs</c>.
/// </summary>
public class TaskDeleteEndpointSpecs : IDisposable
{
    private const string AuthenticatedUserId = "idp-user-123";

    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public TaskDeleteEndpointSpecs()
    {
        this.factory = TaskEndpointTestFactory.Create();
        this.client = this.factory.CreateAuthenticatedClient(AuthenticatedUserId);
    }

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

    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
