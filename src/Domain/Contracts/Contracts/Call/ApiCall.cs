using ExchangeApi.Common.Enums;

namespace ExchangeApi.Contracts.Call;

public sealed record ApiCall<TRequest, TOk, TErr>(
    ExchangeCode Exchange,
    TRequest Request,
    ApiCallMeta Meta,
    ApiCallResult<TOk, TErr> Result);
