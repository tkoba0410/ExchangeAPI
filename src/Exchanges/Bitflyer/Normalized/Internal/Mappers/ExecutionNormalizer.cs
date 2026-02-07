using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class ExecutionNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static bool TryNormalizeList(
        IReadOnlyList<RawPublicDtos.GetExecutionsPublicItem> raw,
        string? rawJson,
        out IReadOnlyList<ExecutionNormalized>? normalized,
        out CallError? error)
    {
        if (raw is null)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer executions response is null.");
            return false;
        }

        var snapshots = ExtractSnapshots(rawJson, raw);
        var mapped = new List<ExecutionNormalized>(raw.Count);
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

    private static bool TryNormalize(
        RawPublicDtos.GetExecutionsPublicItem wire,
        JsonElement snapshot,
        out ExecutionNormalized? normalized,
        out CallError? error)
    {
        if (!SideMapper.TryToExchangeSide(wire.Side, out var side, out error))
        {
            normalized = null;
            return false;
        }

        normalized = new ExecutionNormalized(
            Id: wire.Id,
            Side: side,
            Price: wire.Price,
            Size: wire.Size,
            ExecutedAt: wire.ExecDate,
            ChildOrderAcceptanceId: ToAcceptanceId(wire.ChildOrderAcceptanceId),
            RawSnapshot: snapshot,
            Extras: new Dictionary<FreeText, JsonElement>());
        error = null;
        return true;
    }

    private static AcceptanceId? ToAcceptanceId(string? value) =>
        AcceptanceId.TryParse(value, out var parsed) ? parsed : null;

    private static IReadOnlyList<JsonElement> ExtractSnapshots(
        string? rawJson,
        IReadOnlyList<RawPublicDtos.GetExecutionsPublicItem> raw)
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
