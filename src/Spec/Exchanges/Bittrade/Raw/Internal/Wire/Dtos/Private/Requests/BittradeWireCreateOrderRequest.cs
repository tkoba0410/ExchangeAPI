namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Requests;

internal sealed record BittradeWireCreateOrderRequest(
    string RawSymbol,
    string Side,
    string Type,
    decimal? Price,
    decimal Size);
