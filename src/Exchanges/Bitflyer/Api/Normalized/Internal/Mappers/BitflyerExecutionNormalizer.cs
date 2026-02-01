using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerExecutionNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
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
