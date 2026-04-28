using FluentResults;

namespace BLAInterview.WebApi.Application.Abstractions;

public interface ICommand<TResult>
{
}

public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
