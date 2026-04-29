using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
    public async Task RegistrationController_ReturnsValidationFailure_WhenEmailIsMissing()
    {
        var request = new RegisterUserRequest(
            Name: "Candidate",
            Email: "",
            Password: "Str0ngPassword!");

        var response = await _client.PostAsJsonAsync("/connect/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RegistrationController_ReturnsValidationFailure_WhenEmailFormatIsInvalid()
    {
        var request = new RegisterUserRequest(
            Name: "Candidate",
            Email: "not-an-email",
            Password: "Str0ngPassword!");

        var response = await _client.PostAsJsonAsync("/connect/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RegistrationController_ReturnsValidationFailure_WhenPasswordIsMissing()
    {
        var request = new RegisterUserRequest(
            Name: "Candidate",
            Email: UniqueEmail(),
            Password: "");

        var response = await _client.PostAsJsonAsync("/connect/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RegistrationController_AcceptsRequest_WhenRegistrationDataIsCompleteAndUsable()
    {
        var request = new RegisterUserRequest(
            Name: "Candidate",
            Email: UniqueEmail(),
            Password: "Str0ngPassword!");

        var response = await _client.PostAsJsonAsync("/connect/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(body.GetProperty("userId").GetInt32() > 0);
    }

    private static string UniqueEmail()
    {
        return $"candidate-{Guid.NewGuid():N}@example.com";
    }
}
