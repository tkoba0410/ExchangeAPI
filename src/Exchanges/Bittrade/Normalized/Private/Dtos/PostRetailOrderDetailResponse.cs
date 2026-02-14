namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record PostRetailOrderDetailResponse(
    bool Found,
    RetailOrderEntryNormalized? Item);
