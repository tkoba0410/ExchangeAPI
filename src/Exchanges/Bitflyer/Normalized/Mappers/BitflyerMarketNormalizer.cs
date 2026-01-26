using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using RawPublicModels = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerMarketNormalizer
{
    public static BitflyerMarketNormalized Normalize(RawPublicModels.Market wire) =>
        new(
            ProductCode: wire.ProductCode,
            Alias: wire.Alias);
}
