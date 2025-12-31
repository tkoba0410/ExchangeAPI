using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal interface IBittradeWireMarketDataApi
{
    Task<BittradeWireTicker> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<BittradeWireOrderBook> GetOrderBookAsync(string symbol, CancellationToken ct = default);
}
