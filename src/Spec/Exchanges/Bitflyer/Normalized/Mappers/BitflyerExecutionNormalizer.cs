using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerExecutionNormalizer
{
    public static BitflyerExecutionNormalized Normalize(ExecutionPublicResponse wire) =>
        new(
            Id: wire.Id,
            Side: BitflyerSideMapper.ToExchangeSide(wire.Side),
            Price: wire.Price,
            Size: wire.Size,
            ExecutedAt: wire.ExecDate,
            ChildOrderAcceptanceId: wire.ChildOrderAcceptanceId);
}
