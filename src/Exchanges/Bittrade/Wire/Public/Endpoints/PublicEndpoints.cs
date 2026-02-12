using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Vocabulary;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public.Endpoints;

internal static class PublicEndpoints
{
    public static WireCallSpec GetSymbols() =>
        WireSpecBuilder.Get(EndpointIds.GetSymbols, Paths.CommonSymbolsPath, query: null);

    public static WireCallSpec GetCurrencies() =>
        WireSpecBuilder.Get(EndpointIds.GetCurrencies, Paths.CommonCurrenciesPath, query: null);

    public static WireCallSpec GetTimestamp() =>
        WireSpecBuilder.Get(EndpointIds.GetTimestamp, Paths.CommonTimestampPath, query: null);

    public static WireCallSpec GetHistoryKline(string symbol, string period, string? size = null) =>
        WireSpecBuilder.Get(
            EndpointIds.GetHistoryKline,
            Paths.MarketKlinePath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Period, period),
                (QueryKeys.Symbol, symbol),
                (QueryKeys.Size, size)));

    public static WireCallSpec GetDetailMerged(string symbol) =>
        WireSpecBuilder.Get(
            EndpointIds.GetDetailMerged,
            Paths.MarketMergedPath,
            WireSpecBuilder.BuildQuery((QueryKeys.Symbol, symbol)));

    public static WireCallSpec GetTickers() =>
        WireSpecBuilder.Get(EndpointIds.GetTickers, Paths.MarketTickersPath, query: null);

    public static WireCallSpec GetDepth(string symbol, string? type) =>
        WireSpecBuilder.Get(
            EndpointIds.GetDepth,
            Paths.MarketDepthPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.Symbol, symbol),
                (QueryKeys.Type, type)));

    public static WireCallSpec GetTrade(string symbol) =>
        WireSpecBuilder.Get(
            EndpointIds.GetTrade,
            Paths.MarketTradePath,
            WireSpecBuilder.BuildQuery((QueryKeys.Symbol, symbol)));

    public static WireCallSpec GetHistoryTrade(string symbol) =>
        WireSpecBuilder.Get(
            EndpointIds.GetHistoryTrade,
            Paths.MarketHistoryTradePath,
            WireSpecBuilder.BuildQuery((QueryKeys.Symbol, symbol)));

}
