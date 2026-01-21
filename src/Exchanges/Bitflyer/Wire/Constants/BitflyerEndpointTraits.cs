namespace ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

internal static class BitflyerEndpointTraits
{
    public static bool RequiresAuth(string endpointId)
    {
        return endpointId switch
        {
            BitflyerEndpointIds.GetMarkets => false,
            BitflyerEndpointIds.Markets => false,
            BitflyerEndpointIds.GetBoard => false,
            BitflyerEndpointIds.Board => false,
            BitflyerEndpointIds.GetTicker => false,
            BitflyerEndpointIds.Ticker => false,
            BitflyerEndpointIds.GetExecutionsPublic => false,
            BitflyerEndpointIds.Executions => false,
            BitflyerEndpointIds.GetBoardState => false,
            BitflyerEndpointIds.GetHealth => false,
            BitflyerEndpointIds.GetFundingRate => false,
            BitflyerEndpointIds.GetCorporateLeverage => false,
            BitflyerEndpointIds.GetChats => false,
            _ => true,
        };
    }
}
