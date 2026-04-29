using BLAInterview.Idp.Config;
using BLAInterview.Idp.Authentication;
using BLAInterview.Idp.Data;
using BLAInterview.Idp.Registration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<IdpDbContext>(options =>
    options.UseInMemoryDatabase("BLAInterviewIdp"));

builder.Services
    .AddIdentityServer()
    .AddResourceOwnerValidator<RegisteredUserPasswordValidator>()
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

builder.Services.AddSingleton<PasswordHasher>();

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

app.MapPost("/connect/register", async (
    RegisterUserRequest request,
    IdpDbContext context,
    PasswordHasher passwordHasher,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request.Name)
        || string.IsNullOrWhiteSpace(request.Email)
        || string.IsNullOrWhiteSpace(request.Password)
        || !IsEmailAddress(request.Email))
    {
        return Results.BadRequest();
    }

    var normalizedEmail = request.Email.Trim().ToUpperInvariant();
    if (await context.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken))
    {
        return Results.Conflict();
    }

    var user = new RegisteredUser
    {
        Name = request.Name.Trim(),
        Email = request.Email.Trim(),
        NormalizedEmail = normalizedEmail,
        PasswordHash = passwordHasher.Hash(request.Password),
        CreatedAt = DateTimeOffset.UtcNow
    };

    context.Users.Add(user);
    await context.SaveChangesAsync(cancellationToken);

    return Results.Created($"/connect/users/{user.Id}", new { userId = user.Id });
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

static bool IsEmailAddress(string email)
{
    try
    {
        _ = new MailAddress(email);
        return true;
    }
    catch (FormatException)
    {
        return false;
    }
}

public partial class Program;
