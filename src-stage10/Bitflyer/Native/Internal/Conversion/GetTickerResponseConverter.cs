using System.Text.Json;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Internal.Shared;

namespace ExchangeApi.Stage10.Bitflyer.Native.Internal.Conversion;

internal static class GetTickerResponseConverter
{
    public static bool TryConvert(
        JsonElement root,
        out GetTickerResponseCandidate? candidate,
        out CallError? error)
    {
        if (!JsonScalarReader.TryReadString(root, "product_code", out var productCode, out error) ||
            !JsonScalarReader.TryReadString(root, "state", out var state, out error) ||
            !JsonScalarReader.TryReadDateTimeOffset(root, "timestamp", out var timestamp, out error) ||
            !JsonScalarReader.TryReadInt64(root, "tick_id", out var tickId, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "best_bid", out var bestBid, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "best_ask", out var bestAsk, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "best_bid_size", out var bestBidSize, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "best_ask_size", out var bestAskSize, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "total_bid_depth", out var totalBidDepth, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "total_ask_depth", out var totalAskDepth, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "market_bid_size", out var marketBidSize, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "market_ask_size", out var marketAskSize, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "ltp", out var ltp, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "volume", out var volume, out error) ||
            !JsonScalarReader.TryReadDecimal(root, "volume_by_product", out var volumeByProduct, out error))
        {
            candidate = null;
            return false;
        }

        candidate = new GetTickerResponseCandidate
        {
            ProductCode = productCode,
            State = state,
            Timestamp = timestamp,
            TickId = tickId,
            BestBid = bestBid,
            BestAsk = bestAsk,
            BestBidSize = bestBidSize,
            BestAskSize = bestAskSize,
            TotalBidDepth = totalBidDepth,
            TotalAskDepth = totalAskDepth,
            MarketBidSize = marketBidSize,
            MarketAskSize = marketAskSize,
            Ltp = ltp,
            Volume = volume,
            VolumeByProduct = volumeByProduct,
        };
        error = null;
        return true;
    }
}

internal sealed class GetTickerResponseCandidate
{
    public string? ProductCode { get; init; }

    public string? State { get; init; }

    public DateTimeOffset? Timestamp { get; init; }

    public long? TickId { get; init; }

    public decimal? BestBid { get; init; }

    public decimal? BestAsk { get; init; }

    public decimal? BestBidSize { get; init; }

    public decimal? BestAskSize { get; init; }

    public decimal? TotalBidDepth { get; init; }

    public decimal? TotalAskDepth { get; init; }

    public decimal? MarketBidSize { get; init; }

    public decimal? MarketAskSize { get; init; }

    public decimal? Ltp { get; init; }

    public decimal? Volume { get; init; }

    public decimal? VolumeByProduct { get; init; }
}
