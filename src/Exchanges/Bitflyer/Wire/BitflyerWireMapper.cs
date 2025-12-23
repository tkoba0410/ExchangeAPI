using System;
using System.Collections.Generic;
using System.Linq;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Wire;

internal static class BitflyerWireMapper
{
    public static Ticker MapTicker(Raw.Ticker raw) => new()
    {
        ProductCode = ParseProductCode(raw.ProductCode),
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

    public static Board MapBoard(Raw.Board raw) => new()
    {
        MidPrice = raw.MidPrice,
        Bids = MapBoardEntries(raw.Bids),
        Asks = MapBoardEntries(raw.Asks),
    };

    public static ExecutionPublicResponse MapExecution(Raw.ExecutionPublicResponse raw) => new()
    {
        Id = raw.Id,
        ProductCode = ParseProductCode(raw.ProductCode),
        Side = ParseSideOrThrow(raw.Side, fieldName: "ExecutionPublicResponse.side"),
        Price = raw.Price,
        Size = raw.Size,
        ExecDate = raw.ExecDate,
        ChildOrderAcceptanceId = raw.ChildOrderAcceptanceId,
    };

    public static Market MapMarket(Raw.Market raw) =>
        new(ParseProductCode(raw.ProductCode), raw.Alias);

    public static Chat MapChat(Raw.Chat raw) =>
        new(raw.Nickname, raw.Message, raw.Date);

    public static HealthResponse MapHealth(Raw.HealthResponse raw) =>
        new(raw.Status);

    public static BoardStateResponse MapBoardState(Raw.BoardStateResponse raw) =>
        new(raw.Health, raw.State, raw.Data);

    public static CorporateLeverageResponse MapCorporateLeverage(Raw.CorporateLeverageResponse raw) =>
        new(raw.CurrentMax, raw.CurrentStartDate, raw.NextMax, raw.NextStartDate);

    public static FundingRateResponse MapFundingRate(Raw.FundingRateResponse raw) =>
        new(raw.CurrentFundingRate, raw.NextFundingRateSettleDate);

    private static IReadOnlyList<BoardEntry> MapBoardEntries(IReadOnlyList<Raw.BoardEntry> entries) =>
        entries.Select(entry => new BoardEntry { Price = entry.Price, Size = entry.Size }).ToArray();

    private static ProductCode ParseProductCode(string? value) => value switch
    {
        "BTC_JPY" => ProductCode.BtcJpy,
        "ETH_JPY" => ProductCode.EthJpy,
        "FX_BTC_JPY" => ProductCode.FxBtcJpy,
        _ => ProductCode.Unknown,
    };

    private static Side ParseSideOrThrow(string? value, string fieldName)
    {
        return value switch
        {
            "BUY" => Side.Buy,
            "SELL" => Side.Sell,
            _ => throw new FormatException($"Invalid side '{value}' for {fieldName}."),
        };
    }
}
