using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw アクセス。
/// </summary>
internal interface IBittradePrivateApi
{
    Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<BalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default);

    Task<OpenOrdersResponse> GetOpenOrdersAsync(string symbol, string accountId, CancellationToken cancellationToken = default);

    Task<OrderDetailResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default);

    Task<OrderMatchResultsResponse> GetOrderMatchResultsAsync(string orderId, CancellationToken cancellationToken = default);

    Task<OrdersResponse> GetOrdersAsync(
        string symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<MatchResultsResponse> GetMatchResultsAsync(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<DepositWithdrawsResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default);

    Task<RetailOrdersResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        long? startTime = null,
        long? endTime = null,
        CancellationToken cancellationToken = default);
}
