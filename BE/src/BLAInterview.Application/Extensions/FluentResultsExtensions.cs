using FluentResults;

namespace BLAInterview.Application.Extensions;

/// <summary>
/// Helpers for mapping FluentResults into API-friendly error payloads.
/// </summary>
public static class FluentResultsExtensions
{
    /// <summary>
    /// Projects a result's errors into DTOs. Each error is expected to include a <c>Code</c> entry in metadata.
    /// </summary>
    public static IEnumerable<FluentResultErrorDto> ToErrorDtos(this ResultBase result)
    {
        return result.Errors.Select(error => new FluentResultErrorDto(
            error.Message,
            error.Metadata["Code"]));
    }
}

/// <summary>
/// Error payload returned to clients, with a human message and a machine-readable code.
/// </summary>
public sealed record FluentResultErrorDto(string Message, object Code);
