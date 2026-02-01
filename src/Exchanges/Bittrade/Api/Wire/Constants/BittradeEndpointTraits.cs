namespace ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;

internal static class BittradeEndpointTraits
{
    public static bool RequiresAuth(string endpointId)
    {
        return endpointId switch
        {
            BittradeEndpointIds.GetSymbols => false,
            BittradeEndpointIds.GetCurrencys => false,
            BittradeEndpointIds.GetTimestamp => false,
            BittradeEndpointIds.GetHistoryKline => false,
            BittradeEndpointIds.GetDetailMerged => false,
            BittradeEndpointIds.GetTickers => false,
            BittradeEndpointIds.GetDepth => false,
            BittradeEndpointIds.GetTrade => false,
            BittradeEndpointIds.GetHistoryTrade => false,
            _ => true,
        };
    }
}
