using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerExecutionNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static bool TryNormalizeList(
        IReadOnlyList<RawPublicDtos.ExecutionPublicResponse> raw,
        string? rawJson,
        out IReadOnlyList<BitflyerExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (raw is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer executions response is null.");
            return false;
        }

        var snapshots = ExtractSnapshots(rawJson, raw);
        var mapped = new List<BitflyerExecutionNormalized>(raw.Count);
        for (var i = 0; i < raw.Count; i++)
        {
            if (!TryNormalize(raw[i], snapshots[i], out var execution, out error))
            {
                normalized = null;
                return false;
            }

            mapped.Add(execution!);
        }

        normalized = mapped.ToArray();
        error = null;
        return true;
    }

    public static IReadOnlyList<BitflyerExecutionNormalized> NormalizeList(
        IReadOnlyList<RawPublicDtos.ExecutionPublicResponse> raw,
        string? rawJson)
    {
        var snapshots = ExtractSnapshots(rawJson, raw);
        return raw
            .Select((entry, idx) => Normalize(entry, snapshots[idx]))
            .ToArray();
    }

    private static BitflyerExecutionNormalized Normalize(RawPublicDtos.ExecutionPublicResponse wire, JsonElement snapshot) =>
        new(
            Id: wire.Id,
            Side: BitflyerSideMapper.ToExchangeSide(wire.Side),
            Price: wire.Price,
            Size: wire.Size,
            ExecutedAt: wire.ExecDate,
            ChildOrderAcceptanceId: wire.ChildOrderAcceptanceId,
            RawSnapshot: snapshot,
            Extras: new Dictionary<string, JsonElement>());

    private static bool TryNormalize(
        RawPublicDtos.ExecutionPublicResponse wire,
        JsonElement snapshot,
        out BitflyerExecutionNormalized? normalized,
        out CallError? error)
    {
        if (!BitflyerSideMapper.TryToExchangeSide(wire.Side, out var side, out error))
        {
            normalized = null;
            return false;
        }

        normalized = new BitflyerExecutionNormalized(
            Id: wire.Id,
            Side: side,
            Price: wire.Price,
            Size: wire.Size,
            ExecutedAt: wire.ExecDate,
            ChildOrderAcceptanceId: wire.ChildOrderAcceptanceId,
            RawSnapshot: snapshot,
            Extras: new Dictionary<string, JsonElement>());
        error = null;
        return true;
    }

    private static IReadOnlyList<JsonElement> ExtractSnapshots(
        string? rawJson,
        IReadOnlyList<RawPublicDtos.ExecutionPublicResponse> raw)
    {
        if (raw.Count == 0)
        {
            return Array.Empty<JsonElement>();
        }

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            return raw
                .Select(entry => ExtractSnapshot(Serialize(entry)))
                .ToArray();
        }

        try
        {
            using var doc = JsonDocument.Parse(rawJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
            {
                return raw
                    .Select(entry => ExtractSnapshot(Serialize(entry)))
                    .ToArray();
            }

            var array = doc.RootElement;
            var snapshots = new List<JsonElement>(raw.Count);
            for (var i = 0; i < raw.Count; i++)
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
            return raw
                .Select(entry => ExtractSnapshot(Serialize(entry)))
                .ToArray();
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

    private static string Serialize<T>(T value) =>
        JsonSerializer.Serialize(value, SerializerOptions);
}
