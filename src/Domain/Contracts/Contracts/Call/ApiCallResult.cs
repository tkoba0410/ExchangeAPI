namespace ExchangeApi.Contracts.Call;

public abstract record ApiCallResult<TOk, TErr>(int StatusCode);

public sealed record ApiOk<TOk, TErr>(TOk Value, int StatusCode) : ApiCallResult<TOk, TErr>(StatusCode);

public sealed record ApiErr<TOk, TErr>(TErr Error, int StatusCode) : ApiCallResult<TOk, TErr>(StatusCode);
