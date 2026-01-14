namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

/// <summary>
/// bitFlyer Raw JSON payload wrapper (lossless, no shape assumptions).
/// </summary>
public sealed record RawJsonResponse(string? RawJson);
