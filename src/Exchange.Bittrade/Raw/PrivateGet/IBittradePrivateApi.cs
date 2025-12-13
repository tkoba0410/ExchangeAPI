using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;

namespace ExchangeApi.Adapter.Bittrade;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw アクセス。
/// </summary>
public interface IBittradePrivateApi
{
    Task<BittradeAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<BittradeBalancesResponse> GetBalancesAsync(string accountId, CancellationToken cancellationToken = default);

    Task<BittradeOpenOrdersResponse> GetOpenOrdersAsync(string symbol, string accountId, CancellationToken cancellationToken = default);

    Task<BittradeOrderDetailResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default);
}
