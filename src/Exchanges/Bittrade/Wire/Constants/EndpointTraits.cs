using ExchangeApi.Exchanges.Bittrade.Vocabulary;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Constants;

internal static class EndpointTraits
{
    public static bool RequiresAuth(string endpointId)
    {
        return endpointId switch
        {
            EndpointIds.GetSymbols => false,
            EndpointIds.GetCurrencies => false,
            EndpointIds.GetTimestamp => false,
            EndpointIds.GetHistoryKline => false,
            EndpointIds.GetDetailMerged => false,
            EndpointIds.GetTickers => false,
            EndpointIds.GetDepth => false,
            EndpointIds.GetTrade => false,
            EndpointIds.GetHistoryTrade => false,
            _ => true,
        };
    }
}
