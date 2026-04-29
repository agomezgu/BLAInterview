using System.Net;
using System.Net.Http.Json;
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
    public void TaskCreationEndpoint_AssociatesTaskWithAuthenticatedUser_WhenTaskIsStored()
    {
        Assert.Fail("RED: BE-API-003-T002 not implemented yet.");
    }

    [Fact]
    public void TaskCreationEndpoint_ReturnsValidationFailure_WhenTitleIsMissing()
    {
        Assert.Fail("RED: BE-API-003-T003 not implemented yet.");
    }

    [Fact]
    public void TaskCreationEndpoint_ReturnsCreatedTaskDetails_WhenCreationSucceeds()
    {
        Assert.Fail("RED: BE-API-003-T004 not implemented yet.");
    }

    public void Dispose()
    {
        this.client.Dispose();
        this.factory.Dispose();
    }
}
