using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks.Create;
using BLAInterview.Domain.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:MainDb"] = "Host=localhost;Database=bla_interview_test;Username=test;Password=test"
                    }));

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
                    services.RemoveAll<ITaskRepository>();
                    services.AddSingleton<ITaskRepository, StubTaskRepository>();
                });
            });
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

    private sealed class StubTaskRepository : ITaskRepository
    {
        private readonly List<TaskDto> tasks = [];
        private readonly object syncRoot = new();
        private int nextTaskId;

        public Task<int> AddAsync(TaskEntity task, CancellationToken cancellationToken)
        {
            var taskId = Interlocked.Increment(ref nextTaskId);
            task.Id = taskId;

            lock (syncRoot)
            {
                tasks.Add(new TaskDto(task.Id, task.Title, task.OwnerId, task.Created));
            }

            return Task.FromResult(taskId);
        }

        public Task<IReadOnlyCollection<TaskDto>> GetOwnedTasksAsync(string ownerId, CancellationToken cancellationToken)
        {
            lock (syncRoot)
            {
                IReadOnlyCollection<TaskDto> ownedTasks = tasks
                    .Where(task => task.OwnerId == ownerId)
                    .ToList();

                return Task.FromResult(ownedTasks);
            }
        }

        public Task<TaskDto?> GetOwnedTaskByIdAsync(int taskId, string ownerId, CancellationToken cancellationToken)
        {
            lock (syncRoot)
            {
                var found = tasks.FirstOrDefault(
                    t => t.Id == taskId && t.OwnerId == ownerId);
                return Task.FromResult(found);
            }
        }

        public Task<TaskDto?> UpdateOwnedTaskAsync(
            int taskId,
            string ownerId,
            string? title,
            string? description,
            string? priority,
            string? status,
            CancellationToken cancellationToken)
        {
            lock (syncRoot)
            {
                for (var i = 0; i < tasks.Count; i++)
                {
                    if (tasks[i].Id != taskId || tasks[i].OwnerId != ownerId)
                    {
                        continue;
                    }

                    var current = tasks[i];
                    var newTitle = title ?? current.Title;
                    var newDescription = description ?? current.Description;
                    var newPriority = priority ?? current.Priority;
                    var newStatus = status ?? current.Status;
                    var updated = new TaskDto(
                        current.Id,
                        newTitle,
                        current.OwnerId,
                        current.Created,
                        newDescription,
                        newPriority,
                        newStatus);
                    tasks[i] = updated;
                    return Task.FromResult<TaskDto?>(updated);
                }
            }

            return Task.FromResult<TaskDto?>(null);
        }

        public Task<bool> DeleteOwnedTaskAsync(int taskId, string ownerId, CancellationToken cancellationToken)
        {
            lock (syncRoot)
            {
                for (var i = 0; i < tasks.Count; i++)
                {
                    if (tasks[i].Id == taskId && tasks[i].OwnerId == ownerId)
                    {
                        tasks.RemoveAt(i);
                        return Task.FromResult(true);
                    }
                }
            }

            return Task.FromResult(false);
        }
    }
}
