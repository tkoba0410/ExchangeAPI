namespace ExchangeApi.Spec.CallCommon;

public abstract record CallResult<TOk, TErr>(int StatusCode);

public sealed record Ok<TOk, TErr>(TOk Value, int StatusCode) : CallResult<TOk, TErr>(StatusCode);

public sealed record Err<TOk, TErr>(TErr Error, int StatusCode) : CallResult<TOk, TErr>(StatusCode);
