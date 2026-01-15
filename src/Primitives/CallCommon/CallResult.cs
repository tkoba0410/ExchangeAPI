namespace ExchangeApi.Primitives.CallCommon;

public abstract record CallResult<TRes>
{
    public sealed record Ok(TRes Response) : CallResult<TRes>;

    public sealed record Err(CallError Error) : CallResult<TRes>;
}
