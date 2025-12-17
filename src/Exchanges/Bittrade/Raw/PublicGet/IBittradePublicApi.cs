using System.Threading;
using System.Threading.Tasks;
using Exchange.Bittrade.Raw;
namespace Exchange.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw アクセス（認証不要）。
/// </summary>
public interface IBittradePublicApi
{
    Task<BittradeMergedResponse> GetTickerRawAsync(string symbol, CancellationToken cancellationToken = default);

    Task<BittradeDepthResponse> GetOrderBookRawAsync(string symbol, CancellationToken cancellationToken = default);

    Task<BittradeTradeResponse> GetTradesRawAsync(string symbol, CancellationToken cancellationToken = default);

    Task<BittradeSymbolsResponse> GetSymbolsRawAsync(CancellationToken cancellationToken = default);
}
