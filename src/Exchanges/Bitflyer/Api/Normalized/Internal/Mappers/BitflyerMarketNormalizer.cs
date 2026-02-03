using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerMarketNormalizer
{
    public static BitflyerMarketNormalized Normalize(RawPublicDtos.Market wire) =>
        new(
            ProductCode: ProductCode.ParseNormalized(wire.ProductCode),
            Alias: ParseOptional(wire.Alias));

    private static FreeText? ParseOptional(string? value) =>
        FreeText.TryParse(value, out var text) ? text : null;
}
