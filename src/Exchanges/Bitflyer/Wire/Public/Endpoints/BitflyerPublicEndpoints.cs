using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Public.Endpoints;

internal static class BitflyerPublicEndpoints
{
    public static WireCallSpec GetTicker(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetTicker,
            BitflyerPaths.GetTickerPath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoard(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetBoard,
            BitflyerPaths.GetBoardPath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetExecutionsPublic(
        string productCode,
        string? count = null,
        string? before = null,
        string? after = null)
    {
        return BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetExecutionsPublic,
            BitflyerPaths.GetExecutionsPublicPath,
            BitflyerWireSpecBuilder.BuildQuery(
                (BitflyerQueryKeys.ProductCode, productCode),
                (BitflyerQueryKeys.Count, count),
                (BitflyerQueryKeys.Before, before),
                (BitflyerQueryKeys.After, after)));
    }

    public static WireCallSpec GetMarkets() =>
        BitflyerWireSpecBuilder.Get(BitflyerEndpointIds.GetMarkets, BitflyerPaths.GetMarketsPath, query: null);

    public static WireCallSpec GetChats(string? fromDate = null) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetChats,
            BitflyerPaths.GetChatsPath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.FromDate, fromDate)));

    public static WireCallSpec GetHealth(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetHealth,
            BitflyerPaths.GetHealthPath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetBoardState(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetBoardState,
            BitflyerPaths.GetBoardStatePath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));

    public static WireCallSpec GetCorporateLeverage() =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetCorporateLeverage,
            BitflyerPaths.GetCorporateLeveragePath,
            query: null);

    public static WireCallSpec GetFundingRate(string productCode) =>
        BitflyerWireSpecBuilder.Get(
            BitflyerEndpointIds.GetFundingRate,
            BitflyerPaths.GetFundingRatePath,
            BitflyerWireSpecBuilder.BuildQuery((BitflyerQueryKeys.ProductCode, productCode)));
}
