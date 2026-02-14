using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Constants;

internal static class EndpointTraits
{
    public static bool RequiresAuth(string endpointId)
    {
        return endpointId switch
        {
            EndpointIds.GetMarkets => false,
            EndpointIds.GetBoard => false,
            EndpointIds.GetTicker => false,
            EndpointIds.GetExecutionsPublic => false,
            EndpointIds.GetBoardState => false,
            EndpointIds.GetHealth => false,
            EndpointIds.GetFundingRate => false,
            EndpointIds.GetCorporateLeverage => false,
            EndpointIds.GetChats => false,
            _ => true,
        };
    }
}
