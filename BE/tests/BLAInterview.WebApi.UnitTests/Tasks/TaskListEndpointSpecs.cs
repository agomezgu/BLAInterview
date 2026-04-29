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
        Assert.Equal("Prepare interview notes", tasks[0].Title);
        Assert.Equal(AuthenticatedUserId, tasks[0].OwnerId);
    }

    [Fact]
    public void TaskListEndpoint_ReturnsEmptyList_WhenAuthenticatedUserHasNoTasks()
    {
        Assert.Fail("RED: BE-API-004-T002 not implemented yet.");
    }

    [Fact]
    public void TaskListEndpoint_ExcludesOtherUsersTasks_WhenAuthenticatedUserRequestsList()
    {
        Assert.Fail("RED: BE-API-004-T003 not implemented yet.");
    }

    [Fact]
    public void TaskListEndpoint_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        Assert.Fail("RED: BE-API-004-T004 not implemented yet.");
    }

    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
