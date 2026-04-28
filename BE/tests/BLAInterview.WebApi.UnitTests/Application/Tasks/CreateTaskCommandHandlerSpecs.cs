using BLAInterview.Application.Abstractions;
using BLAInterview.Application.Tasks;
using BLAInterview.Application.Tasks.Create;

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
        ICommandHandler<CreateTaskCommand, int> handler =
            new CreateTaskCommandHandler(validator);

        // Act
        FluentResults.Result<int> result =
            await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotEqual(0, result.Value);
        
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
