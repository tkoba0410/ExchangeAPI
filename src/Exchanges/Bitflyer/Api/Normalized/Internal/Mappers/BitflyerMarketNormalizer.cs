using System;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerMarketNormalizer
{
    public static bool TryNormalize(
        RawPublicDtos.Market wire,
        out BitflyerMarketNormalized? normalized,
        out CallError? error)
    {
        try
        {
            normalized = Build(wire);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer market response invalid.", ex);
            return false;
        }
    }

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;

    private static BitflyerMarketNormalized Build(RawPublicDtos.Market wire) =>
        new(
            ProductCode: ProductCode.ParseNormalized(wire.ProductCode),
            Alias: ParseOptional(wire.Alias));
}
