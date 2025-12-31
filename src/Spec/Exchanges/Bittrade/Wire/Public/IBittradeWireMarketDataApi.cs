using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal interface IBittradeWireMarketDataApi
{
    Task<WireResponse<BittradeWireTicker>> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<WireResponse<BittradeWireOrderBook>> GetOrderBookAsync(string symbol, CancellationToken ct = default);
}
