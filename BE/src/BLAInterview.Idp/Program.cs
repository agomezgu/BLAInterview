using BLAInterview.Idp.Data;
using BLAInterview.Idp.Registration;
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

public partial class Program;
