namespace ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;

public sealed record BittradeWireCreateOrderRequest(
    string Symbol,
    string Side,
    string Type,
    decimal? Price,
    decimal Size);
