using BLAInterview.Idp.Config;
using BLAInterview.Idp.Data;
using BLAInterview.Idp.Registration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<IdpDbContext>(options =>
    options.UseInMemoryDatabase("BLAInterviewIdp"));

builder.Services
    .AddIdentityServer()
    .AddConfigurationStore(options =>
    {
        options.ConfigureDbContext = db =>
            db.UseInMemoryDatabase("BLAInterviewIdpConfiguration");
    })
    .AddOperationalStore(options =>
    {
        options.ConfigureDbContext = db =>
            db.UseInMemoryDatabase("BLAInterviewIdpOperational");
    })
    .AddDeveloperSigningCredential();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

SeedIdentityServerConfiguration(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.MapPost("/connect/register", (RegisterUserRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest();
    }

    return Results.Accepted();
});

app.Run();

static void SeedIdentityServerConfiguration(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    if (!context.ApiResources.Any())
    {
        context.ApiResources.AddRange(Config.ApiResources.Select(apiResource => apiResource.ToEntity()));
    }

    if (!context.ApiScopes.Any())
    {
        context.ApiScopes.AddRange(Config.ApiScopes.Select(apiScope => apiScope.ToEntity()));
    }

    if (!context.Clients.Any())
    {
        context.Clients.AddRange(Config.Clients.Select(client => client.ToEntity()));
    }

    context.SaveChanges();
}

public partial class Program;
