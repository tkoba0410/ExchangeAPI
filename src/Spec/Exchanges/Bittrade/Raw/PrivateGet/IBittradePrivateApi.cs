using System;
using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw アクセス。
/// </summary>
internal interface IBittradePrivateApi
{
    Task<RawAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default);

    Task<RawBalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default);

    Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default);

    Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default);

    Task<RawOrderMatchResultsResponse> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default);

    Task<RawOrdersResponse> GetOrdersAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<RawMatchResultsResponse> GetMatchResultsAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<RawDepositWithdrawsResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default);

    Task<RawRetailOrdersResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default);
}
