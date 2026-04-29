using System.Security.Claims;
using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Application.Tasks.List;
using BLAInterview.Infrastructure.Tasks;
using FluentValidation;
using Npgsql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var authenticationSection = builder.Configuration.GetSection("Authentication");
var apiAudience = authenticationSection["Audience"] ?? "bla-interview-api";

// Add services to the container.

var connectionString =
    builder.Configuration.GetConnectionString("MainDb")
    ?? throw new InvalidOperationException("Connection string 'MainDb' not found.");

builder.Services.AddSingleton(sp =>
{
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
    return dataSourceBuilder.Build();
});

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
builder.Services.AddScoped<IValidator<CreateTaskCommand>, CreateTaskCommandValidator>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ICommandHandler<CreateTaskCommand, int>, CreateTaskCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ListOwnedTasksQuery, IReadOnlyCollection<TaskDto>>, ListOwnedTasksQueryHandler>();
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
