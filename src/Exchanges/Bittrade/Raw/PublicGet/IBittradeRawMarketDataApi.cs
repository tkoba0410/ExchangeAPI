using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawMarketDataApi
{
    Task<MergedResponse> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default);
    Task<DepthResponse> GetOrderBookAsync(Symbol symbol, string? type = null, CancellationToken cancellationToken = default);
}
