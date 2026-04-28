using FluentResults;

namespace BLAInterview.Application.Extensions;

public static class FluentResultsExtensions
{
    public static IEnumerable<FluentResultErrorDto> ToErrorDtos(this ResultBase result)
    {
        return result.Errors.Select(error => new FluentResultErrorDto(
            error.Message,
            error.Metadata["Code"]));
    }
}

public sealed record FluentResultErrorDto(string Message, object Code);
