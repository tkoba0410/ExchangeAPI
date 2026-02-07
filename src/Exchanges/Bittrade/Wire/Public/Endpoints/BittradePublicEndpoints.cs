using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public.Endpoints;

internal static class BittradePublicEndpoints
{
    public static WireCallSpec GetSymbols() =>
        BittradeWireSpecBuilder.Get(BittradeEndpointIds.GetSymbols, BittradePaths.CommonSymbolsPath, query: null);

    public static WireCallSpec GetCurrencys() =>
        BittradeWireSpecBuilder.Get(BittradeEndpointIds.GetCurrencys, BittradePaths.CommonCurrenciesPath, query: null);

    public static WireCallSpec GetTimestamp() =>
        BittradeWireSpecBuilder.Get(BittradeEndpointIds.GetTimestamp, BittradePaths.CommonTimestampPath, query: null);

    public static WireCallSpec GetHistoryKline(string symbol, string period, string? size = null) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetHistoryKline,
            BittradePaths.MarketKlinePath,
            BittradeWireSpecBuilder.BuildQuery(
                (BittradeQueryKeys.Period, period),
                (BittradeQueryKeys.Symbol, symbol),
                (BittradeQueryKeys.Size, size)));

    public static WireCallSpec GetDetailMerged(string symbol) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetDetailMerged,
            BittradePaths.MarketMergedPath,
            BittradeWireSpecBuilder.BuildQuery((BittradeQueryKeys.Symbol, symbol)));

    public static WireCallSpec GetTickers() =>
        BittradeWireSpecBuilder.Get(BittradeEndpointIds.GetTickers, BittradePaths.MarketTickersPath, query: null);

    public static WireCallSpec GetDepth(string symbol, string? type) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetDepth,
            BittradePaths.MarketDepthPath,
            BittradeWireSpecBuilder.BuildQuery(
                (BittradeQueryKeys.Symbol, symbol),
                (BittradeQueryKeys.Type, type)));

    public static WireCallSpec GetTrade(string symbol) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetTrade,
            BittradePaths.MarketTradePath,
            BittradeWireSpecBuilder.BuildQuery((BittradeQueryKeys.Symbol, symbol)));

    public static WireCallSpec GetHistoryTrade(string symbol) =>
        BittradeWireSpecBuilder.Get(
            BittradeEndpointIds.GetHistoryTrade,
            BittradePaths.MarketHistoryTradePath,
            BittradeWireSpecBuilder.BuildQuery((BittradeQueryKeys.Symbol, symbol)));

}
