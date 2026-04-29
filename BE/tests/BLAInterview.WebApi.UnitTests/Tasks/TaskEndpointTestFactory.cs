using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BLAInterview.WebApi.UnitTests.Tasks;

internal static class TaskEndpointTestFactory
{
    private const string Issuer = "https://idp.test";
    private const string Audience = "bla-interview-api";
    private const string Scope = "bla-interview-api";
    private static readonly SymmetricSecurityKey SigningKey = new(Encoding.UTF8.GetBytes("test-signing-key-with-enough-length"));

    public static WebApplicationFactory<Program> Create()
    {
        return new WebApplicationFactory<Program>()
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
    }

    public static HttpClient CreateAuthenticatedClient(this WebApplicationFactory<Program> factory, string userId)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            CreateBearerToken(userId));

        return client;
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
