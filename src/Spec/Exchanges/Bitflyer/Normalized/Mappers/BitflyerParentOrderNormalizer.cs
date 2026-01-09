using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Spec.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerParentOrderNormalizer
{
    public static IReadOnlyList<BitflyerParentOrderNormalized> NormalizeList(
        IReadOnlyList<ParentOrderResponse> raw,
        string? rawJson)
    {
        var snapshots = ExtractSnapshots(rawJson, raw.Count);
        return raw
            .Select((item, idx) => NormalizeSummary(item, snapshots[idx]))
            .ToArray();
    }

    public static BitflyerParentOrderDetailNormalized NormalizeDetail(
        ParentOrderDetailResponse raw,
        string? rawJson)
    {
        var snapshot = ExtractSnapshot(rawJson);
        var parameters = raw.Parameters
            .Select(NormalizeParameter)
            .ToArray();

        return new BitflyerParentOrderDetailNormalized(
            Id: raw.Id,
            ParentOrderId: raw.ParentOrderId,
            OrderMethod: ParseOrderMethod(raw.OrderMethod),
            ExpireDate: raw.ExpireDate,
            TimeInForce: ParseTimeInForce(raw.TimeInForce),
            Parameters: parameters,
            ParentOrderAcceptanceId: raw.ParentOrderAcceptanceId,
            RawSnapshot: snapshot,
            Extras: new Dictionary<string, JsonElement>());
    }

    private static BitflyerParentOrderNormalized NormalizeSummary(
        ParentOrderResponse raw,
        JsonElement snapshot)
    {
        return new BitflyerParentOrderNormalized(
            Id: raw.Id,
            ParentOrderId: raw.ParentOrderId,
            ProductCode: raw.ProductCode,
            Side: ParseSide(raw.Side),
            ParentOrderType: ParseParentOrderType(raw.ParentOrderType),
            Price: raw.Price == 0 ? null : new Price(raw.Price),
            AveragePrice: raw.AveragePrice == 0 ? null : new Price(raw.AveragePrice),
            Size: new Size(raw.Size),
            ParentOrderState: ParseParentOrderState(raw.ParentOrderState),
            ExpireDate: raw.ExpireDate,
            ParentOrderDate: raw.ParentOrderDate,
            ParentOrderAcceptanceId: raw.ParentOrderAcceptanceId,
            OutstandingSize: new Size(raw.OutstandingSize),
            CancelSize: new Size(raw.CancelSize),
            ExecutedSize: new Size(raw.ExecutedSize),
            TotalCommission: raw.TotalCommission,
            RawSnapshot: snapshot,
            Extras: new Dictionary<string, JsonElement>());
    }

    private static BitflyerParentOrderParameterNormalized NormalizeParameter(ParentOrderParameterResponse raw)
    {
        return new BitflyerParentOrderParameterNormalized(
            ProductCode: raw.ProductCode,
            ConditionType: ParseConditionType(raw.ConditionType),
            Side: ParseSide(raw.Side),
            Price: raw.Price is > 0 ? new Price(raw.Price.Value) : null,
            Size: raw.Size is > 0 ? new Size(raw.Size.Value) : null,
            TriggerPrice: raw.TriggerPrice is > 0 ? new Price(raw.TriggerPrice.Value) : null,
            Offset: raw.Offset is > 0 ? raw.Offset : null);
    }

    private static Closed<BitflyerOrderMethod> ParseOrderMethod(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "SIMPLE" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.Simple),
            "IFD" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.Ifd),
            "OCO" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.Oco),
            "IFDOCO" => Closed<BitflyerOrderMethod>.KnownValue(BitflyerOrderMethod.IfdOco),
            _ => Closed<BitflyerOrderMethod>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<BitflyerTimeInForce> ParseTimeInForce(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "GTC" => Closed<BitflyerTimeInForce>.KnownValue(BitflyerTimeInForce.Gtc),
            "IOC" => Closed<BitflyerTimeInForce>.KnownValue(BitflyerTimeInForce.Ioc),
            "FOK" => Closed<BitflyerTimeInForce>.KnownValue(BitflyerTimeInForce.Fok),
            _ => Closed<BitflyerTimeInForce>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<BitflyerSide> ParseSide(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "BUY" => Closed<BitflyerSide>.KnownValue(BitflyerSide.Buy),
            "SELL" => Closed<BitflyerSide>.KnownValue(BitflyerSide.Sell),
            _ => Closed<BitflyerSide>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<BitflyerParentOrderState> ParseParentOrderState(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "ACTIVE" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Active),
            "COMPLETED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Completed),
            "CANCELED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Canceled),
            "EXPIRED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Expired),
            "REJECTED" => Closed<BitflyerParentOrderState>.KnownValue(BitflyerParentOrderState.Rejected),
            _ => Closed<BitflyerParentOrderState>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<BitflyerParentOrderType> ParseParentOrderType(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "SIMPLE" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Simple),
            "IFD" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Ifd),
            "OCO" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Oco),
            "IFDOCO" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.IfdOco),
            "LIMIT" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Limit),
            "MARKET" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Market),
            "STOP" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Stop),
            "STOP_LIMIT" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.StopLimit),
            "TRAIL" => Closed<BitflyerParentOrderType>.KnownValue(BitflyerParentOrderType.Trail),
            _ => Closed<BitflyerParentOrderType>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<BitflyerConditionType> ParseConditionType(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "LIMIT" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Limit),
            "MARKET" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Market),
            "STOP" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Stop),
            "STOP_LIMIT" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.StopLimit),
            "TRAIL" => Closed<BitflyerConditionType>.KnownValue(BitflyerConditionType.Trail),
            _ => Closed<BitflyerConditionType>.UnknownValue(raw ?? string.Empty),
        };

    private static IReadOnlyList<JsonElement> ExtractSnapshots(string? rawJson, int count)
    {
        if (count == 0)
        {
            return Array.Empty<JsonElement>();
        }

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return Enumerable.Range(0, count).Select(_ => EmptySnapshot()).ToArray();
        }

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return Enumerable.Range(0, count).Select(_ => EmptySnapshot()).ToArray();
            }

            var array = doc.RootElement;
            var snapshots = new List<JsonElement>(count);
            for (var i = 0; i < count; i++)
            {
                if (i < array.GetArrayLength())
                {
                    snapshots.Add(array[i].Clone());
                }
                else
                {
                    snapshots.Add(EmptySnapshot());
                }
            }

            return snapshots;
        }
        catch (JsonException)
        {
            return Enumerable.Range(0, count).Select(_ => EmptySnapshot()).ToArray();
        }
    }

    private static JsonElement ExtractSnapshot(string? rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return EmptySnapshot();
        }

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            return doc.RootElement.Clone();
        }
        catch (JsonException)
        {
            return EmptySnapshot();
        }
    }

    private static JsonElement EmptySnapshot()
    {
        using var doc = JsonDocument.Parse("{}");
        return doc.RootElement.Clone();
    }
}
