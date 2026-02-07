using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BitflyerTickerNormalizer
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static bool TryNormalize(
        RawPublicDtos.GetTickerResponse wire,
        string? rawJson,
        out BitflyerTickerNormalized? normalized,
        out CallError? error)
    {
        try
        {
            normalized = Build(wire, rawJson);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer ticker response invalid.", ex);
            return false;
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

    private static BitflyerTickerNormalized Build(RawPublicDtos.GetTickerResponse wire, string? rawJson)
    {
        var snapshot = ExtractSnapshot(rawJson ?? Serialize(wire));
        return new BitflyerTickerNormalized(
            ProductCode: ProductCode.ParseNormalized(wire.ProductCode),
            LastTradedPrice: wire.LastTradedPrice,
            Timestamp: wire.Timestamp,
            RawSnapshot: snapshot,
            Extras: new Dictionary<FreeText, JsonElement>());
    }
}
