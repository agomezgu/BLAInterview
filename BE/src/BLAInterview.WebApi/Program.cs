using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var authenticationSection = builder.Configuration.GetSection("Authentication");
var apiAudience = authenticationSection["Audience"] ?? "bla-interview-api";

// Add services to the container.

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authenticationSection["Authority"];
        options.Audience = apiAudience;
        options.RequireHttpsMetadata = authenticationSection.GetValue("RequireHttpsMetadata", true);
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        // .RequireClaim("sub")
        .RequireAssertion(context => HasApiScope(context.User, apiAudience))
        .Build();
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok()).AllowAnonymous();
app.MapControllers();

app.Run();

static bool HasApiScope(ClaimsPrincipal user, string apiAudience)
{
    return user.FindAll("scope")
        .SelectMany(claim => claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        .Contains(apiAudience, StringComparer.Ordinal);
}

public partial class Program;
