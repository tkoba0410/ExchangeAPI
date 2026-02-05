namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;

/// <summary>
/// bitFlyer Raw JSON payload wrapper (lossless, no shape assumptions).
/// </summary>
public sealed record RawJsonResponse(string? RawJson);
