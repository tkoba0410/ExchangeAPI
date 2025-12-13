using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bittrade.RawApi;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Adapter.Bittrade;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw 実装。
/// </summary>
public sealed class BittradePrivateApi : IBittradePrivateApi
{
    private readonly IRestClient _restClient;

    public BittradePrivateApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<BittradeAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        return _restClient.GetAsync<BittradeAccountsResponse>(
            "v1/account/accounts",
            cancellationToken: cancellationToken);
    }

    public Task<BittradeBalancesResponse> GetBalancesAsync(string accountId, CancellationToken cancellationToken = default)
    {
        EnsureAccountId(accountId);
        return _restClient.GetAsync<BittradeBalancesResponse>(
            $"v1/account/accounts/{accountId}/balance",
            cancellationToken: cancellationToken);
    }

    public Task<BittradeOpenOrdersResponse> GetOpenOrdersAsync(string symbol, string accountId, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        EnsureAccountId(accountId);

        return _restClient.GetAsync<BittradeOpenOrdersResponse>(
            $"v1/order/openOrders?symbol={ToApiSymbol(symbol)}&account-id={accountId}",
            cancellationToken: cancellationToken);
    }

    public Task<BittradeOrderDetailResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new ArgumentException("orderId is required.", nameof(orderId));
        }

        return _restClient.GetAsync<BittradeOrderDetailResponse>(
            $"v1/order/orders/{orderId}",
            cancellationToken: cancellationToken);
    }

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", "", StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }

    private static void EnsureAccountId(string accountId)
    {
        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }
    }
}
