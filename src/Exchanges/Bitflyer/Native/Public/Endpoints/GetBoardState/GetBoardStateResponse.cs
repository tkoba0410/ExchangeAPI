namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;

public sealed class GetBoardStateResponse
{
    public required string Health { get; init; }
    public required string State { get; init; }
    public GetBoardStateData? Data { get; init; }
}

public sealed class GetBoardStateData
{
    public decimal? SpecialQuotation { get; init; }
}
