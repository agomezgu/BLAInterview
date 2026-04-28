namespace BLAInterview.IntegrationTests.Authentication;

public class TaskEndpointAuthenticationSpecs
{
    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenAuthenticationTokenIsMissing()
    {
        Assert.Fail("RED: BE-API-001-T001 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        Assert.Fail("RED: BE-API-001-T002 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenTokenIsExpired()
    {
        Assert.Fail("RED: BE-API-001-T003 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_ReturnsUnauthorized_WhenTokenIssuerIsIncorrect()
    {
        Assert.Fail("RED: BE-API-001-T004 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_AllowsRequest_WhenIdpTokenIncludesUserIdentity()
    {
        Assert.Fail("RED: BE-API-001-T005 not implemented yet.");
    }

    [Fact]
    public void TaskEndpoint_UsesIdpUserIdentifier_WhenProcessingTaskRequest()
    {
        Assert.Fail("RED: BE-API-001-T006 not implemented yet.");
    }
}
