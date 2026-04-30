using BLAInterview.Idp.Authentication;
using BLAInterview.Idp.Config;
using BLAInterview.Idp.Data;
using BLAInterview.Idp.Registration;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();
builder.Services.AddDbContext<IdpDbContext>(options =>
    options.UseInMemoryDatabase("BLAInterviewIdp"));


builder.Services
    .AddIdentityServer(options =>
    {
        options.KeyManagement.Enabled = false;
        options.UserInteraction.LoginUrl = "/Account/Login";
        options.UserInteraction.LogoutUrl = "/Account/Logout";
        options.UserInteraction.ErrorUrl = "/home/error";
    })
    .AddResourceOwnerValidator<RegisteredUserPasswordValidator>()
    .AddProfileService<RegisteredUserProfileService>()
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
builder.Services.AddScoped<RegisteredUserRegistrar>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

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
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();
app.MapControllers();


app.MapPost("/connect/register", async (
    RegisterUserRequest request,
    RegisteredUserRegistrar registrar,
    CancellationToken cancellationToken) =>
{
    var (outcome, userId) = await registrar.RegisterAsync(request, cancellationToken);
    return outcome switch
    {
        RegisterUserOutcome.Created => Results.Created($"/connect/users/{userId}", new { userId }),
        RegisterUserOutcome.DuplicateEmail => Results.Conflict(),
        _ => Results.BadRequest()
    };

});



app.Run();



static void SeedIdentityServerConfiguration(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

    if (!context.IdentityResources.Any())
    {
        context.IdentityResources.AddRange(Config.IdentityResources.Select(r => r.ToEntity()));
    }
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

