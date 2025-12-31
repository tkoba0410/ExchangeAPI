using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public.Models;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

public interface IBittradeWireMarketDataApi
{
    Task<WireResponse> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<WireResponse> GetOrderBookAsync(string symbol, string? type = null, CancellationToken ct = default);
    Task<WireResponse> GetTradesAsync(string symbol, CancellationToken ct = default);
    Task<WireResponse> GetKlinesAsync(string symbol, string period, int? size = null, CancellationToken ct = default);
    Task<WireResponse> GetTickersAsync(CancellationToken ct = default);
    Task<WireResponse> GetTradeHistoryAsync(string symbol, CancellationToken ct = default);
}
