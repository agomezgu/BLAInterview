using FluentResults;

namespace BLAInterview.Application.Abstractions;

/// <summary>
/// Marker interface for application commands/queries that return <typeparamref name="TResult"/>.
/// </summary>
public interface ICommand<TResult>
{
}

/// <summary>
/// Handles an application command/query and returns a <see cref="Result{TValue}"/> representing success or failure.
/// </summary>
public interface ICommandHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Executes the command/query and returns a success value or failure errors.
    /// </summary>
    Task<Result<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
