namespace BLAInterview.Infrastructure.UnitTests.Tasks;

/// <summary>
/// BE-API-007: task delete persistence against PostgreSQL. Remove <c>Fact(Skip=...)</c> and
/// implement with <see cref="BLAInterview.Infrastructure.Tasks.TaskRepository"/> (or extend fixture) when going green.
/// </summary>
public class DeleteTaskSpecs
{
    [Fact(Skip = "BE-API-007: TDD red — add DeleteOwnedTaskAsync (or equivalent) and assert row removal when going green.")]
    public async Task TaskRepository_RemovesTask_WhenOwnerDeletesOwnedTask()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — add delete-by-owner and assert other user cannot delete when going green.")]
    public async Task TaskRepository_DoesNotDelete_WhenOwnerIdDoesNotMatch()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — add delete and assert no row affected for unknown id when going green.")]
    public async Task TaskRepository_DoesNotDelete_WhenTaskIdDoesNotExist()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — assert task absent from list after delete when going green.")]
    public async Task TaskRepository_TaskNotInOwnedList_AfterOwnerDeletesTask()
    {
        await Task.CompletedTask;
    }
}
