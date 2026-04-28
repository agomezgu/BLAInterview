using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace BLAInterview.WebApi.UnitTests.Authentication;

public class TaskEndpointAuthenticationSpecs
{
    [Fact]
    public async Task TaskEndpoint_ReturnsUnauthorized_WhenAuthenticationTokenIsMissing()
    {
        const string protectedTaskRoute = "/tasks";
        using var factory = new WebApplicationFactory<Program>();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(protectedTaskRoute);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TaskEndpoint_IdentifyUser_WhenProcessingTaskRequest()
    {
        const string idpIssuedUserId = "idp-user-123";
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-signing-key-with-enough-length"));
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = "https://idp.test",
                        ValidateAudience = true,
                        ValidAudience = "bla-interview-api",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateLifetime = true
                    };
                });
            }));
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken(idpIssuedUserId, signingKey));

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(idpIssuedUserId, await response.Content.ReadAsStringAsync());
    }

    private static string CreateBearerToken(string userId, SecurityKey signingKey)
    {
        var token = new JwtSecurityToken(
            issuer: "https://idp.test",
            audience: "bla-interview-api",
            claims: [new Claim("sub", userId)],
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
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
