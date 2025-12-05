namespace ExchangeApi.Core.Dtos;

/// <summary>
/// オープンな子注文の概要。
/// </summary>
public sealed record OpenOrder(
    string ProductCode,
    string OrderId,
    string OrderAcceptanceId,
    OrderSide Side,
    OrderType OrderType,
    decimal Size,
    decimal OutstandingSize,
    decimal ExecutedSize,
    decimal? Price,
    string? ClientOrderId = null);
