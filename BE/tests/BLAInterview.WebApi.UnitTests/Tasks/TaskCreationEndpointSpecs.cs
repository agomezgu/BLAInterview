using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BLAInterview.WebApi.UnitTests.Tasks;

public class TaskCreationEndpointSpecs : IDisposable
{
    private const string Issuer = "https://idp.test";
    private const string Audience = "bla-interview-api";
    private const string Scope = "bla-interview-api";
    private static readonly SymmetricSecurityKey SigningKey = new(Encoding.UTF8.GetBytes("test-signing-key-with-enough-length"));

    private readonly WebApplicationFactory<Program> factory;
    private readonly HttpClient client;
    private readonly object request;

    public TaskCreationEndpointSpecs()
    {
        this.factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
                builder.ConfigureServices(services =>
                {
                    services.Configure<JwtBearerOptions>(
                        JwtBearerDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Authority = string.Empty;
                            options.MetadataAddress = string.Empty;
                            options.MapInboundClaims = false;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = Issuer,
                                ValidateAudience = true,
                                ValidAudience = Audience,
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = SigningKey,
                                ValidateLifetime = true,
                                ClockSkew = TimeSpan.Zero
                            };
                        });
                }));
        this.client = this.factory.CreateClient();
        this.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken("idp-user-123"));
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

    private static string CreateBearerToken(string userId)
    {
        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims:
            [
                new Claim("sub", userId),
                new Claim("scope", Scope)
            ],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
