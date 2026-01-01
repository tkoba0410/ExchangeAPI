using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

public interface IBittradeWireMarketDataApi
{
    Task<WireCall> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<WireCall> GetOrderBookAsync(string symbol, string? type = null, CancellationToken ct = default);
    Task<WireCall> GetTradesAsync(string symbol, CancellationToken ct = default);
    Task<WireCall> GetKlinesAsync(string symbol, string period, int? size = null, CancellationToken ct = default);
    Task<WireCall> GetTickersAsync(CancellationToken ct = default);
    Task<WireCall> GetTradeHistoryAsync(string symbol, CancellationToken ct = default);
}
