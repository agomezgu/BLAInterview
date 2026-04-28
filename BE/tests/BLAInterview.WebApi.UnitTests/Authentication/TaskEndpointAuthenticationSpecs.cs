using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;


namespace BLAInterview.WebApi.UnitTests.Authentication;

public class TaskEndpointAuthenticationSpecs
{
    [Fact]
    public async Task TaskEndpoint_ReturnsUnauthorized_WhenAuthenticationTokenIsMissing()
    {
        const string protectedTaskRoute = "/tasks";
        var programType = Type.GetType("Program, BLAInterview.WebApi", throwOnError: true)!;
        using var factory = (IDisposable)Activator.CreateInstance(
            typeof(WebApplicationFactory<>).MakeGenericType(programType))!;
        using var client = (HttpClient)factory
            .GetType()
            .GetMethod(nameof(WebApplicationFactory<object>.CreateClient), Type.EmptyTypes)!
            .Invoke(factory, [])!;

        var response = await client.GetAsync(protectedTaskRoute);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public void TaskEndpoint_IdentifyUser_WhenProcessingTaskRequest()
    {
        // Arrange
        const string idpIssuedUserId = "idp-user-123";
        var controller = new Controllers.TasksController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(
                            [
                                new Claim("sub", idpIssuedUserId)
                            ],
                            authenticationType: "TestAuthentication"))
                }
            }
        };

        // Act
        var result = controller.GetTasks();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseJson = JsonSerializer.Serialize(okResult.Value);
        Assert.Contains(idpIssuedUserId, responseJson);
    }

    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        Assert.Fail("RED: BE-API-001-T002 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenTokenIsExpired()
    {
        Assert.Fail("RED: BE-API-001-T003 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenTokenIssuerIsIncorrect()
    {
        Assert.Fail("RED: BE-API-001-T004 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_AllowsRequest_WhenIdpTokenIncludesUserIdentity()
    {
        Assert.Fail("RED: BE-API-001-T005 not implemented yet.");
    }
}
