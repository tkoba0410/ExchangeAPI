namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record GetRetailOrderDetailByOrderIdResponse(
    bool Found,
    RetailOrderEntryNormalized? Item);
