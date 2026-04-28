using BLAInterview.WebApi.Application.Abstractions;
using BLAInterview.WebApi.Application.Tasks;

namespace BLAInterview.WebApi.UnitTests.Application.Tasks;

public class CreateTaskCommandHandlerSpecs
{
    [Fact]
    public async Task CreateTaskCommandHandler_CreatesTask_WhenCommandHasTitleAndOwner()
    {
        // Arrange
        var command = new CreateTaskCommand(
            Title: "Prepare interview notes",
            OwnerId: "idp-user-123");
        FluentValidation.IValidator<CreateTaskCommand> validator =
            new CreateTaskCommandValidator();
        ICommandHandler<CreateTaskCommand, CreateTaskResult> handler =
            new CreateTaskCommandHandler(validator);

        // Act
        FluentResults.Result<CreateTaskResult> result =
            await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Equal("Prepare interview notes", result.Value.Title);
        Assert.Equal("idp-user-123", result.Value.OwnerId);
    }

    [Fact]
    public void CreateTaskCommandHandler_AssignsOwnerId_WhenTaskIsStored()
    {
        Assert.Fail("RED: BE-API-003-T002 not implemented yet.");
    }

    [Fact]
    public void CreateTaskCommandHandler_ReturnsValidationFailure_WhenTitleIsMissing()
    {
        Assert.Fail("RED: BE-API-003-T003 not implemented yet.");
    }

    [Fact]
    public void CreateTaskCommandHandler_ReturnsCreatedTaskDetails_WhenTaskIsCreated()
    {
        Assert.Fail("RED: BE-API-003-T004 not implemented yet.");
    }
}
