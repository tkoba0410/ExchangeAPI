using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateApi : IBittradePrivateApi
{
    private readonly IRestClient _restClient;

    public BittradePrivateApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<AccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetAsync<AccountsResponse>("v1/account/accounts", cancellationToken: cancellationToken);

    public Task<BalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(accountId, nameof(accountId));
        return _restClient.GetAsync<BalancesResponse>(
            $"v1/account/accounts/{accountId}/balance",
            cancellationToken: cancellationToken);
    }

    public Task<OpenOrdersResponse> GetOpenOrdersAsync(string symbol, string accountId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(symbol, nameof(symbol));
        EnsureRequired(accountId, nameof(accountId));
        var query = BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("account-id", accountId));

        return _restClient.GetAsync<OpenOrdersResponse>(
            $"v1/order/openOrders?{query}",
            cancellationToken: cancellationToken);
    }

    public Task<OrderDetailResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId, nameof(orderId));
        return _restClient.GetAsync<OrderDetailResponse>(
            $"v1/order/orders/{orderId}",
            cancellationToken: cancellationToken);
    }

    public Task<OrderMatchResultsResponse> GetOrderMatchResultsAsync(string orderId, CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId, nameof(orderId));
        return _restClient.GetAsync<OrderMatchResultsResponse>(
            $"v1/order/orders/{orderId}/matchresults",
            cancellationToken: cancellationToken);
    }

    public Task<OrdersResponse> GetOrdersAsync(
        string symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(symbol, nameof(symbol));
        EnsureRequired(states, nameof(states));

        var query = BuildQuery(
            ("symbol", ToApiSymbol(symbol)),
            ("states", states),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString()));

        return _restClient.GetAsync<OrdersResponse>(
            $"v1/order/orders?{query}",
            cancellationToken: cancellationToken);
    }

    public Task<MatchResultsResponse> GetMatchResultsAsync(
        string? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(
            ("symbol", string.IsNullOrWhiteSpace(symbol) ? null : ToApiSymbol(symbol)),
            ("types", types),
            ("start-date", startDate),
            ("end-date", endDate),
            ("from", from?.ToString()),
            ("direct", direct),
            ("size", size?.ToString()));

        return _restClient.GetAsync<MatchResultsResponse>(
            $"v1/order/matchresults?{query}",
            cancellationToken: cancellationToken);
    }

    public Task<DepositWithdrawsResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(type, nameof(type));
        var query = BuildQuery(
            ("type", type),
            ("currency", currency),
            ("from", from?.ToString()),
            ("size", size?.ToString()),
            ("direct", direct));

        return _restClient.GetAsync<DepositWithdrawsResponse>(
            $"v1/query/deposit-withdraw?{query}",
            cancellationToken: cancellationToken);
    }

    public Task<RetailOrdersResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var startTimeMs = startTime?.ToUnixTimeMilliseconds();
        var endTimeMs = endTime?.ToUnixTimeMilliseconds();
        var query = BuildQuery(
            ("direct", direct.ToString()),
            ("status", status?.ToString()),
            ("start_time", startTimeMs?.ToString()),
            ("end_time", endTimeMs?.ToString()));

        return _restClient.GetAsync<RetailOrdersResponse>(
            $"v1/retail/order/list?{query}",
            cancellationToken: cancellationToken);
    }

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureRequired(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} is required.", name);
        }
    }

    private static string BuildQuery(params (string Key, string? Value)[] items)
    {
        var parts = items
            .Where(i => !string.IsNullOrWhiteSpace(i.Value))
            .Select(i => $"{i.Key}={Uri.EscapeDataString(i.Value!)}");
        return string.Join("&", parts);
    }
}
