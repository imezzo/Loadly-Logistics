namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IAction<TRequest, TResponse>
{
    Task<TResponse> ExecuteAsync(TRequest request);
}
public interface IAction<TResponse>
{
    Task<TResponse> ExecuteAsync();
}
public interface IVoidAction<TRequest>
{
    Task ExecuteAsync(TRequest request);
}
