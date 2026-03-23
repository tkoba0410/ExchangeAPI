namespace ExchangeApi.Primitives.Calls;

public sealed class Call<TRequest, TResponse>
{
    public required TRequest Request { get; init; }
    public required TResponse? Response { get; init; }
    public required bool IsSuccess { get; init; }
    public required CallError? Error { get; init; }
    public required CallMeta Meta { get; init; }
}
