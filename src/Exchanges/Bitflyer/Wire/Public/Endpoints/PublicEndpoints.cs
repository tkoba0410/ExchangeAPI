using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Public.Endpoints;

internal static class PublicEndpoints
{
    public static WireCallSpec GetTicker(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetTicker,
            Paths.GetTickerPath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoard(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetBoard,
            Paths.GetBoardPath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetExecutionsPublic(
        string productCode,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return WireSpecBuilder.Get(
            EndpointIds.GetExecutionsPublic,
            Paths.GetExecutionsPublicPath,
            WireSpecBuilder.BuildQuery(
                (QueryKeys.ProductCode, productCode),
                (QueryKeys.Count, count),
                (QueryKeys.Before, before),
                (QueryKeys.After, after)));
    }

    public static WireCallSpec GetMarkets() =>
        WireSpecBuilder.Get(EndpointIds.GetMarkets, Paths.GetMarketsPath, query: null);

    public static WireCallSpec GetChats(string? fromDate = null) =>
        WireSpecBuilder.Get(
            EndpointIds.GetChats,
            Paths.GetChatsPath,
            WireSpecBuilder.BuildQuery((QueryKeys.FromDate, fromDate)));

    public static WireCallSpec GetHealth(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetHealth,
            Paths.GetHealthPath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoardState(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetBoardState,
            Paths.GetBoardStatePath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCorporateLeverage() =>
        WireSpecBuilder.Get(
            EndpointIds.GetCorporateLeverage,
            Paths.GetCorporateLeveragePath,
            query: null);

    public static WireCallSpec GetFundingRate(string productCode) =>
        WireSpecBuilder.Get(
            EndpointIds.GetFundingRate,
            Paths.GetFundingRatePath,
            WireSpecBuilder.BuildQuery((QueryKeys.ProductCode, productCode)));
}
