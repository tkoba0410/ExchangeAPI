namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

public sealed record GetAddressesResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetCoinInsResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetCoinOutsResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetBankAccountsResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetDepositsResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetWithdrawalsResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetBalanceHistoryResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetCollateralHistoryResponse(string? RawJson) : RawJsonResponse(RawJson);

public sealed record GetTradingCommissionResponse(string? RawJson) : RawJsonResponse(RawJson);
