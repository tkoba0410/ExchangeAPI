using System;
using System.Collections.Generic;
using System.Linq;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using WirePublic = ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Converters;

internal static class BitflyerWireMapper
{
    public static WirePublic.Ticker MapTicker(Raw.Ticker raw) => new()
    {
        ProductCode = raw.ProductCode,
        Timestamp = raw.Timestamp,
        TickId = raw.TickId,
        BestBid = raw.BestBid,
        BestAsk = raw.BestAsk,
        BestBidSize = raw.BestBidSize,
        BestAskSize = raw.BestAskSize,
        TotalBidDepth = raw.TotalBidDepth,
        TotalAskDepth = raw.TotalAskDepth,
        LastTradedPrice = raw.LastTradedPrice,
        Volume = raw.Volume,
        VolumeByProduct = raw.VolumeByProduct,
    };

    public static WirePublic.Board MapBoard(Raw.Board raw) => new()
    {
        MidPrice = raw.MidPrice,
        Bids = MapBoardEntries(raw.Bids),
        Asks = MapBoardEntries(raw.Asks),
    };

    public static WirePublic.ExecutionPublicResponse MapExecution(Raw.ExecutionPublicResponse raw) => new()
    {
        Id = raw.Id,
        ProductCode = raw.ProductCode,
        Side = ParseSideOrThrow(raw.Side, fieldName: "ExecutionPublicResponse.side"),
        Price = raw.Price,
        Size = raw.Size,
        ExecDate = raw.ExecDate,
        ChildOrderAcceptanceId = raw.ChildOrderAcceptanceId,
    };

    public static WirePublic.Market MapMarket(Raw.Market raw) =>
        new(raw.ProductCode, raw.Alias);

    public static WirePublic.Chat MapChat(Raw.Chat raw) =>
        new(raw.Nickname, raw.Message, raw.Date);

    public static WirePublic.HealthResponse MapHealth(Raw.HealthResponse raw) =>
        new(raw.Status);

    public static WirePublic.BoardStateResponse MapBoardState(Raw.BoardStateResponse raw) =>
        new(raw.Health, raw.State, raw.Data);

    public static WirePublic.CorporateLeverageResponse MapCorporateLeverage(Raw.CorporateLeverageResponse raw) =>
        new(raw.CurrentMax, raw.CurrentStartDate, raw.NextMax, raw.NextStartDate);

    public static WirePublic.FundingRateResponse MapFundingRate(Raw.FundingRateResponse raw) =>
        new(raw.CurrentFundingRate, raw.NextFundingRateSettleDate);

    private static IReadOnlyList<WirePublic.BoardEntry> MapBoardEntries(IReadOnlyList<Raw.BoardEntry> entries) =>
        entries.Select(entry => new WirePublic.BoardEntry { Price = entry.Price, Size = entry.Size }).ToArray();

    private static Raw.Side ParseSideOrThrow(string? value, string fieldName)
    {
        return value switch
        {
            "BUY" => Raw.Side.Buy,
            "SELL" => Raw.Side.Sell,
            _ => throw new FormatException($"Invalid side '{value}' for {fieldName}."),
        };
    }
}
