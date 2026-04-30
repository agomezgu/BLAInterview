namespace BLAInterview.WebApi.UnitTests.Tasks;

/// <summary>
/// BE-API-007: HTTP delete for an owned task. Skipped until the route and handler exist; use
/// <c>TaskEndpointTestFactory</c> + authenticated client like <c>TaskUpdateEndpointSpecs</c> when going green.
/// </summary>
public class TaskDeleteEndpointSpecs
{
    [Fact(Skip = "BE-API-007: TDD red — add DELETE /tasks/{id} and assert success when going green.")]
    public async Task TaskDeleteEndpoint_ReturnsSuccess_WhenUserDeletesOwnedTask()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — add route and assert 404 when other user’s task when going green.")]
    public async Task TaskDeleteEndpoint_ReturnsNotFound_WhenTaskOwnedByAnotherUser()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — add route and assert 404 for missing id when going green.")]
    public async Task TaskDeleteEndpoint_ReturnsNotFound_WhenTaskIdDoesNotExist()
    {
        await Task.CompletedTask;
    }

    [Fact(Skip = "BE-API-007: TDD red — delete then GET same id returns 404 when going green.")]
    public async Task TaskDeleteEndpoint_GetTaskReturnsNotFound_AfterSuccessfulDelete()
    {
        await Task.CompletedTask;
    }
}
