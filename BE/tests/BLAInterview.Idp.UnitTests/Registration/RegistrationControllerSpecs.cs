using System.Net;
using System.Net.Http.Json;
using BLAInterview.Idp.Registration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BLAInterview.Idp.UnitTests.Registration;

public class RegistrationControllerSpecs(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task RegistrationController_ReturnsValidationFailure_WhenNameIsMissing()
    {
        const string registrationRoute = "/connect/register";
        const HttpStatusCode validationFailureStatusCode = HttpStatusCode.BadRequest;
        var request = new RegisterUserRequest(
            Name: "",
            Email: "candidate@example.com",
            Password: "Str0ngPassword!");

        var response = await _client.PostAsJsonAsync(registrationRoute, request);

        Assert.Equal(validationFailureStatusCode, response.StatusCode);
    }

    [Fact]
    public void RegistrationController_ReturnsValidationFailure_WhenEmailIsMissing()
    {
        Assert.Fail("RED: BE-IDP-001-T002 not implemented yet.");
    }

    [Fact]
    public void RegistrationController_ReturnsValidationFailure_WhenEmailFormatIsInvalid()
    {
        Assert.Fail("RED: BE-IDP-001-T003 not implemented yet.");
    }

    [Fact]
    public void RegistrationController_ReturnsValidationFailure_WhenPasswordIsMissing()
    {
        Assert.Fail("RED: BE-IDP-001-T004 not implemented yet.");
    }

    [Fact]
    public void RegistrationController_AcceptsRequest_WhenRegistrationDataIsCompleteAndUsable()
    {
        Assert.Fail("RED: BE-IDP-001-T005 not implemented yet.");
    }
}
