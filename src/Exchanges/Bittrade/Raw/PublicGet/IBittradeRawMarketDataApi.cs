using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawMarketDataApi
{
    Task<RawMergedResponse> GetTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default);
    Task<RawDepthResponse> GetOrderBookAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default);
    Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default);
}
