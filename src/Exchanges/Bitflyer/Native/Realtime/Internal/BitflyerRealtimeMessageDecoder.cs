using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;

internal static class BitflyerRealtimeMessageDecoder
{
    internal static BitflyerRealtimeTickerMessage DecodeTicker(BitflyerRealtimeChannelMessage message)
    {
        EnsureObject(message.Message, message.Channel);

        return new BitflyerRealtimeTickerMessage
        {
            Channel = message.Channel,
            ReceivedAt = message.ReceivedAt,
            ProductCode = JsonValueReader.ReadRequiredString(message.Message, "product_code"),
            Timestamp = JsonValueReader.ReadRequiredUtcTimestamp(message.Message, "timestamp"),
            TickId = JsonValueReader.ReadRequiredLong(message.Message, "tick_id"),
            BestBid = JsonValueReader.ReadRequiredDecimal(message.Message, "best_bid"),
            BestAsk = JsonValueReader.ReadRequiredDecimal(message.Message, "best_ask"),
            BestBidSize = JsonValueReader.ReadRequiredDecimal(message.Message, "best_bid_size"),
            BestAskSize = JsonValueReader.ReadRequiredDecimal(message.Message, "best_ask_size"),
            TotalBidDepth = JsonValueReader.ReadRequiredDecimal(message.Message, "total_bid_depth"),
            TotalAskDepth = JsonValueReader.ReadRequiredDecimal(message.Message, "total_ask_depth"),
            Ltp = JsonValueReader.ReadRequiredDecimal(message.Message, "ltp"),
            Volume = JsonValueReader.ReadRequiredDecimal(message.Message, "volume"),
            VolumeByProduct = JsonValueReader.ReadRequiredDecimal(message.Message, "volume_by_product"),
        };
    }

    internal static IReadOnlyList<BitflyerRealtimeExecutionMessage> DecodeExecutions(
        BitflyerRealtimeChannelMessage message,
        string productCode)
    {
        if (message.Message.ValueKind != JsonValueKind.Array)
        {
            throw new CodecException("Realtime executions payload must be an array.");
        }

        var executions = new List<BitflyerRealtimeExecutionMessage>();
        foreach (var item in message.Message.EnumerateArray())
        {
            EnsureObject(item, message.Channel);
            executions.Add(new BitflyerRealtimeExecutionMessage
            {
                Channel = message.Channel,
                ReceivedAt = message.ReceivedAt,
                ProductCode = productCode,
                Id = JsonValueReader.ReadRequiredLong(item, "id"),
                Side = JsonValueReader.ReadRequiredString(item, "side"),
                Price = JsonValueReader.ReadRequiredDecimal(item, "price"),
                Size = JsonValueReader.ReadRequiredDecimal(item, "size"),
                ExecDate = JsonValueReader.ReadRequiredUtcTimestamp(item, "exec_date"),
                BuyChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "buy_child_order_acceptance_id"),
                SellChildOrderAcceptanceId = JsonValueReader.ReadRequiredString(item, "sell_child_order_acceptance_id"),
            });
        }

        return executions;
    }

    internal static BitflyerRealtimeBoardSnapshotMessage DecodeBoardSnapshot(
        BitflyerRealtimeChannelMessage message,
        string productCode)
    {
        EnsureObject(message.Message, message.Channel);

        return new BitflyerRealtimeBoardSnapshotMessage
        {
            Channel = message.Channel,
            ReceivedAt = message.ReceivedAt,
            ProductCode = productCode,
            MidPrice = JsonValueReader.ReadRequiredDecimal(message.Message, "mid_price"),
            Bids = ReadLevels(message.Message, "bids"),
            Asks = ReadLevels(message.Message, "asks"),
        };
    }

    internal static BitflyerRealtimeBoardDeltaMessage DecodeBoardDelta(
        BitflyerRealtimeChannelMessage message,
        string productCode)
    {
        EnsureObject(message.Message, message.Channel);

        return new BitflyerRealtimeBoardDeltaMessage
        {
            Channel = message.Channel,
            ReceivedAt = message.ReceivedAt,
            ProductCode = productCode,
            MidPrice = JsonValueReader.ReadRequiredDecimal(message.Message, "mid_price"),
            Bids = ReadLevels(message.Message, "bids"),
            Asks = ReadLevels(message.Message, "asks"),
        };
    }

    private static IReadOnlyList<BitflyerRealtimeBoardLevel> ReadLevels(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array)
        {
            throw new CodecException($"Property '{propertyName}' must be an array.");
        }

        var levels = new List<BitflyerRealtimeBoardLevel>();
        foreach (var level in property.EnumerateArray())
        {
            EnsureObject(level, propertyName);
            levels.Add(new BitflyerRealtimeBoardLevel
            {
                Price = JsonValueReader.ReadRequiredDecimal(level, "price"),
                Size = JsonValueReader.ReadRequiredDecimal(level, "size"),
            });
        }

        return levels;
    }

    private static void EnsureObject(JsonElement element, string label)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            throw new CodecException($"Realtime payload '{label}' must be an object.");
        }
    }
}
