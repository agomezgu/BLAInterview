using System.Net;
using System.Net.Http.Json;
using BLAInterview.Application.Tasks.Create;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BLAInterview.WebApi.UnitTests.Tasks;

public class TaskListEndpointSpecs : IDisposable
{
    private const string AuthenticatedUserId = "idp-user-123";

    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;

    public TaskListEndpointSpecs()
    {
        this.factory = TaskEndpointTestFactory.Create();
        this.client = this.factory.CreateAuthenticatedClient(AuthenticatedUserId);
    }

    [Fact]
    public async Task TaskListEndpoint_ReturnsOwnedTasks_WhenAuthenticatedUserHasTasks()
    {
        var creationResponse = await client.PostAsJsonAsync(
            "/tasks",
            new CreateTaskDto("Prepare interview notes"));
        Assert.Equal(HttpStatusCode.Created, creationResponse.StatusCode);

        var response = await this.client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        Assert.NotNull(tasks);

        Assert.Single(tasks);
        Assert.True(tasks[0].Id > 0);
        Assert.Equal("Prepare interview notes", tasks[0].Title);
        Assert.Equal(AuthenticatedUserId, tasks[0].OwnerId);
        Assert.NotEqual(default, tasks[0].Created);
    }

    [Fact]
    public async Task TaskListEndpoint_ReturnsEmptyList_WhenAuthenticatedUserHasNoTasks()
    {
        var response = await this.client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task TaskListEndpoint_ExcludesOtherUsersTasks_WhenAuthenticatedUserRequestsList()
    {
        var otherUserClient = this.factory.CreateAuthenticatedClient("idp-user-456");
        try
        {
            var ownedCreationResponse = await client.PostAsJsonAsync(
                "/tasks",
                new CreateTaskDto("Prepare interview notes"));
            var otherCreationResponse = await otherUserClient.PostAsJsonAsync(
                "/tasks",
                new CreateTaskDto("Review scorecard"));
            Assert.Equal(HttpStatusCode.Created, ownedCreationResponse.StatusCode);
            Assert.Equal(HttpStatusCode.Created, otherCreationResponse.StatusCode);

            var response = await this.client.GetAsync("/tasks");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var tasks = await response.Content.ReadFromJsonAsync<List<TaskDto>>();
            Assert.NotNull(tasks);
            var task = Assert.Single(tasks);
            Assert.Equal("Prepare interview notes", task.Title);
            Assert.Equal(AuthenticatedUserId, task.OwnerId);
        }
        finally
        {
            otherUserClient.Dispose();
        }
    }

    [Fact]
    public async Task TaskListEndpoint_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        using var unauthenticatedClient = this.factory.CreateClient();

        var response = await unauthenticatedClient.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
