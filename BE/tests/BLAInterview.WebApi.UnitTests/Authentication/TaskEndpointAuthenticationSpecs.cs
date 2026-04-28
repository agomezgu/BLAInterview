using System.Net;
using System.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace BLAInterview.WebApi.UnitTests.Authentication;

public class TaskEndpointAuthenticationSpecs
{
    private const string Issuer = "https://idp.test";
    private const string Audience = "bla-interview-api";
    private const string Scope = "bla-interview-api";
    private static readonly SymmetricSecurityKey SigningKey = new(Encoding.UTF8.GetBytes("test-signing-key-with-enough-length"));

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
        using var factory = CreateFactory(SigningKey);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken(idpIssuedUserId, SigningKey));

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
    }

    [Fact]
    public async Task TaskEndpoint_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        var untrustedSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("untrusted-signing-key-with-enough-length"));
        using var factory = CreateFactory(SigningKey);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken("idp-user-123", untrustedSigningKey));

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TaskEndpoint_ReturnsUnauthorized_WhenTokenIsExpired()
    {
        using var factory = CreateFactory(SigningKey);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken("idp-user-123", SigningKey, expires: DateTime.UtcNow.AddMinutes(-1)));

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TaskEndpoint_ReturnsUnauthorized_WhenTokenIssuerIsIncorrect()
    {
        using var factory = CreateFactory(SigningKey);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken("idp-user-123", SigningKey, issuer: "https://unexpected-idp.test"));

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task TaskEndpoint_AllowsRequest_WhenIdpTokenIncludesUserIdentity()
    {
        using var factory = CreateFactory(SigningKey);
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken("idp-user-456", SigningKey));

        var response = await client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory(SecurityKey signingKey)
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services =>
            {
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
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
                        IssuerSigningKey = signingKey,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
            }));
    }

    private static string CreateBearerToken(
        string userId,
        SecurityKey signingKey,
        string issuer = Issuer,
        DateTime? expires = null)
    {
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: Audience,
            claims:
            [
                new Claim("sub", userId),
                new Claim("scope", Scope)
            ],
            expires: expires ?? DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
