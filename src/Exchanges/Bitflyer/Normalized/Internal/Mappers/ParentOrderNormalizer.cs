using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using RawPrivateDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;
using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class ParentOrderNormalizer
{
    public static bool TryNormalizeList(
        IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse> raw,
        string? rawJson,
        out IReadOnlyList<ParentOrderNormalized>? normalized,
        out CallError? error)
    {
        if (raw is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer parent orders response is null.");
            return false;
        }

        try
        {
            normalized = BuildList(raw, rawJson);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer parent orders response invalid.", ex);
            return false;
        }
    }

    public static bool TryNormalizeDetail(
        RawPrivateDtos.GetParentOrderResponse raw,
        string? rawJson,
        out ParentOrderDetailNormalized? normalized,
        out CallError? error)
    {
        try
        {
            normalized = BuildDetail(raw, rawJson);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer parent order response invalid.", ex);
            return false;
        }
    }

    private static IReadOnlyList<ParentOrderNormalized> BuildList(
        IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse> raw,
        string? rawJson)
    {
        var snapshots = ExtractSnapshots(rawJson, raw.Count);
        return raw
            .Select((item, idx) => NormalizeSummary(item, snapshots[idx]))
            .ToArray();
    }

    private static ParentOrderDetailNormalized BuildDetail(
        RawPrivateDtos.GetParentOrderResponse raw,
        string? rawJson)
    {
        var snapshot = ExtractSnapshot(rawJson);
        var parameters = raw.Parameters
            .Select(NormalizeParameter)
            .ToArray();

        return new ParentOrderDetailNormalized(
            Id: raw.Id,
            ParentOrderId: ExchangeOrderId.Parse(raw.ParentOrderId),
            OrderMethod: ParseOrderMethod(raw.OrderMethod),
            ExpireDate: raw.ExpireDate,
            TimeInForce: ParseTimeInForce(raw.TimeInForce),
            Parameters: parameters,
            ParentOrderAcceptanceId: AcceptanceId.Parse(raw.ParentOrderAcceptanceId),
            RawSnapshot: snapshot,
            Extras: new Dictionary<FreeText, JsonElement>());
    }

    private static ParentOrderNormalized NormalizeSummary(
        RawPrivateDtos.RawGetParentOrdersResponse raw,
        JsonElement snapshot)
    {
        return new ParentOrderNormalized(
            Id: raw.Id,
            ParentOrderId: ExchangeOrderId.Parse(raw.ParentOrderId),
            ProductCode: ProductCode.ParseNormalized(raw.ProductCode),
            Side: ParseSide(raw.Side),
            ParentOrderType: ParseParentOrderType(raw.ParentOrderType),
            Price: raw.Price == 0 ? null : new Price(raw.Price),
            AveragePrice: raw.AveragePrice == 0 ? null : new Price(raw.AveragePrice),
            Size: new Size(raw.Size),
            ParentOrderState: ParseParentOrderState(raw.ParentOrderState),
            ExpireDate: raw.ExpireDate,
            ParentOrderDate: raw.ParentOrderDate,
            ParentOrderAcceptanceId: AcceptanceId.Parse(raw.ParentOrderAcceptanceId),
            OutstandingSize: new Size(raw.OutstandingSize),
            CancelSize: new Size(raw.CancelSize),
            ExecutedSize: new Size(raw.ExecutedSize),
            TotalCommission: raw.TotalCommission,
            RawSnapshot: snapshot,
            Extras: new Dictionary<FreeText, JsonElement>());
    }

    private static ParentOrderParameterNormalized NormalizeParameter(RawPrivateDtos.ParentOrderParameterResponse raw)
    {
        return new ParentOrderParameterNormalized(
            ProductCode: ProductCode.ParseNormalized(raw.ProductCode),
            ConditionType: ParseConditionType(raw.ConditionType),
            Side: ParseSide(raw.Side),
            Price: raw.Price is > 0 ? new Price(raw.Price.Value) : null,
            Size: raw.Size is > 0 ? new Size(raw.Size.Value) : null,
            TriggerPrice: raw.TriggerPrice is > 0 ? new Price(raw.TriggerPrice.Value) : null,
            Offset: raw.Offset is > 0 ? raw.Offset : null);
    }

    private static Closed<OrderMethod> ParseOrderMethod(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "SIMPLE" => Closed<OrderMethod>.KnownValue(OrderMethod.Simple),
            "IFD" => Closed<OrderMethod>.KnownValue(OrderMethod.Ifd),
            "OCO" => Closed<OrderMethod>.KnownValue(OrderMethod.Oco),
            "IFDOCO" => Closed<OrderMethod>.KnownValue(OrderMethod.IfdOco),
            _ => Closed<OrderMethod>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<TimeInForce> ParseTimeInForce(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "GTC" => Closed<TimeInForce>.KnownValue(TimeInForce.Gtc),
            "IOC" => Closed<TimeInForce>.KnownValue(TimeInForce.Ioc),
            "FOK" => Closed<TimeInForce>.KnownValue(TimeInForce.Fok),
            _ => Closed<TimeInForce>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<ExchangeSide> ParseSide(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "BUY" => Closed<ExchangeSide>.KnownValue(ExchangeSide.Buy),
            "SELL" => Closed<ExchangeSide>.KnownValue(ExchangeSide.Sell),
            _ => Closed<ExchangeSide>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<ParentOrderState> ParseParentOrderState(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "ACTIVE" => Closed<ParentOrderState>.KnownValue(ParentOrderState.Active),
            "COMPLETED" => Closed<ParentOrderState>.KnownValue(ParentOrderState.Completed),
            "CANCELED" => Closed<ParentOrderState>.KnownValue(ParentOrderState.Canceled),
            "EXPIRED" => Closed<ParentOrderState>.KnownValue(ParentOrderState.Expired),
            "REJECTED" => Closed<ParentOrderState>.KnownValue(ParentOrderState.Rejected),
            _ => Closed<ParentOrderState>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<ParentOrderType> ParseParentOrderType(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "SIMPLE" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Simple),
            "IFD" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Ifd),
            "OCO" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Oco),
            "IFDOCO" => Closed<ParentOrderType>.KnownValue(ParentOrderType.IfdOco),
            "LIMIT" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Limit),
            "MARKET" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Market),
            "STOP" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Stop),
            "STOP_LIMIT" => Closed<ParentOrderType>.KnownValue(ParentOrderType.StopLimit),
            "TRAIL" => Closed<ParentOrderType>.KnownValue(ParentOrderType.Trail),
            _ => Closed<ParentOrderType>.UnknownValue(raw ?? string.Empty),
        };

    private static Closed<ConditionType> ParseConditionType(string raw) =>
        (raw ?? string.Empty).ToUpperInvariant() switch
        {
            "LIMIT" => Closed<ConditionType>.KnownValue(ConditionType.Limit),
            "MARKET" => Closed<ConditionType>.KnownValue(ConditionType.Market),
            "STOP" => Closed<ConditionType>.KnownValue(ConditionType.Stop),
            "STOP_LIMIT" => Closed<ConditionType>.KnownValue(ConditionType.StopLimit),
            "TRAIL" => Closed<ConditionType>.KnownValue(ConditionType.Trail),
            _ => Closed<ConditionType>.UnknownValue(raw ?? string.Empty),
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
