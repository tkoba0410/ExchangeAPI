namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

/// <summary>
/// bitFlyer Raw JSON payload wrapper (lossless, no shape assumptions).
/// </summary>
public record RawJsonResponse(string? RawJson);
