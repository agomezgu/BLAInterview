using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BLAInterview.Application.Tasks.Create;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BLAInterview.WebApi.UnitTests.Tasks;

public class TaskCreationEndpointSpecs : IDisposable
{
    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;
    private readonly object request;

    public TaskCreationEndpointSpecs()
    {
        this.factory = TaskEndpointTestFactory.Create();
        this.client = this.factory.CreateAuthenticatedClient("idp-user-123");
        this.request = new { title = "Prepare interview notes" };
    }

    [Fact]
    public async Task TaskCreationEndpoint_CreatesTask_WhenAuthenticatedUserSubmitsTitle()
    {
        // Act
        var response = await this.client.PostAsJsonAsync("/tasks", this.request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task TaskCreationEndpoint_AssociatesTaskWithAuthenticatedUser_WhenTaskIsStored()
    {
        var creationResponse = await this.client.PostAsJsonAsync("/tasks", this.request);
        Assert.Equal(HttpStatusCode.Created, creationResponse.StatusCode);

        var listResponse = await this.client.GetAsync("/tasks");
        var tasks = await listResponse.Content.ReadFromJsonAsync<List<TaskDto>>();

        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        Assert.NotNull(tasks);
        var task = Assert.Single(tasks);
        Assert.Equal("idp-user-123", task.OwnerId);
    }

    [Fact]
    public async Task TaskCreationEndpoint_ReturnsValidationFailure_WhenTitleIsMissing()
    {
        var response = await this.client.PostAsJsonAsync("/tasks", new { title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("TASK_TITLE_REQUIRED", errors[0].GetProperty("code").GetString());
    }

    [Fact]
    public async Task TaskCreationEndpoint_ReturnsCreatedTaskDetails_WhenCreationSucceeds()
    {
        var response = await this.client.PostAsJsonAsync("/tasks", this.request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("taskId").GetInt32() > 0);
        Assert.Equal("TASK_CREATED", body.GetProperty("code").GetString());
    }

    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
