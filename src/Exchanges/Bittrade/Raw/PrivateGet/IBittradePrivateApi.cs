using System.Threading;
using System.Threading.Tasks;
using Exchange.Bittrade.Raw;
namespace Exchange.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw アクセス。
/// </summary>
public interface IBittradePrivateApi
{
    Task<BittradeAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<BittradeBalancesResponse> GetBalancesAsync(string accountId, CancellationToken cancellationToken = default);

    Task<BittradeOpenOrdersResponse> GetOrdersAsync(string symbol, string accountId, CancellationToken cancellationToken = default);

    Task<BittradeOrderDetailResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default);
}
