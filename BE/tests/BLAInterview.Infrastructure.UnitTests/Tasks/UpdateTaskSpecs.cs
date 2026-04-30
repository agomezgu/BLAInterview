using BLAInterview.Infrastructure.UnitTests.Fixtures;
using Xunit;

namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// Specifies task update persistence against the PostgreSQL fixture.
/// </summary>
public class UpdateTaskSpecs : IClassFixture<LocalPostgresFixture>, IAsyncLifetime
{
    private readonly LocalPostgresFixture _fixture;

    public UpdateTaskSpecs(LocalPostgresFixture fixture)
    {
        _fixture = fixture;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public Task InitializeAsync() => _fixture.ResetAsync();

    [Fact]
    public void TaskRepository_PersistsUpdatedFields_WhenOwnerUpdatesTask()
    {
        // RED: BE-API-006-T010 not implemented yet.
    }

    [Fact]
    public void TaskRepository_DoesNotExposeOtherUserTask_WhenUpdateTargetsWrongOwner()
    {
        // RED: BE-API-006-T011 not implemented yet.
    }
}
