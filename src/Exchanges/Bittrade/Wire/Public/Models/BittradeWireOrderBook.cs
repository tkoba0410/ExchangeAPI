using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public.Models;

public sealed record BittradeWireOrderBook(
    IReadOnlyList<BittradeWirePriceSize> Bids,
    IReadOnlyList<BittradeWirePriceSize> Asks
);

public sealed record BittradeWirePriceSize(decimal Price, decimal Size);
