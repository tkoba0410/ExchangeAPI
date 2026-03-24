namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;

public sealed class GetFundingRateResponse
{
    public required decimal CurrentFundingRate { get; init; }
    public required DateTimeOffset NextFundingRateSettleDate { get; init; }
}
