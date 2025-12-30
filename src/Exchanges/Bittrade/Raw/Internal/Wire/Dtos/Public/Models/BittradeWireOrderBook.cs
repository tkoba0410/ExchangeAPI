using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public.Models;

internal sealed record BittradeWireOrderBook(
    IReadOnlyList<BittradeWirePriceSize> Bids,
    IReadOnlyList<BittradeWirePriceSize> Asks
);

internal sealed record BittradeWirePriceSize(decimal Price, decimal Size);
