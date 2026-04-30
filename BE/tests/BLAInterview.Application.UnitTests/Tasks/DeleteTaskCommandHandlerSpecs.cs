namespace BLAInterview.Application.UnitTests.Tasks;

/// <summary>
/// BE-API-007: <c>DeleteTaskCommand</c> + handler + <see cref="BLAInterview.Application.Abstractions.ITaskRepository"/>
/// delete contract. Tests are skipped until the types exist; remove <c>Fact(Skip=...)</c> and implement when going green.
/// </summary>
public class DeleteTaskCommandHandlerSpecs
{
    [Fact(Skip = "BE-API-007: TDD red — add DeleteTaskCommand, handler, and repository method when going green.")]
    public async Task DeleteTaskCommandHandler_ReturnsSuccess_WhenUserDeletesOwnedTask()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — add DeleteTaskCommand, handler, and repository method when going green.")]
    public async Task DeleteTaskCommandHandler_ReturnsNotFound_WhenTaskOwnedByAnotherUser()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — add DeleteTaskCommand, handler, and repository method when going green.")]
    public async Task DeleteTaskCommandHandler_ReturnsNotFound_WhenTaskIdDoesNotExist()
    {
        await Task.CompletedTask;
    }
}
