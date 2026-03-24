namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;

public sealed class GetCorporateLeverageResponse
{
    public required decimal CurrentMax { get; init; }
    public required DateTimeOffset CurrentStartDate { get; init; }
    public decimal? NextMax { get; init; }
    public DateTimeOffset? NextStartDate { get; init; }
}
