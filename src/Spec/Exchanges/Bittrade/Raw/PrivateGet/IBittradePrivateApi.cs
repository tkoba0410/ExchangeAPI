using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
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

    Task<BittradeRawCall<RawAccountsResponse, JsonElement>> GetAccountsCallAsync(
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawBalancesResponse, JsonElement>> GetAccountBalanceCallAsync(
        string accountId,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawOpenOrdersResponse, JsonElement>> GetOpenOrdersCallAsync(
        RawSymbol symbol,
        string accountId,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawOrderDetailResponse, JsonElement>> GetOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawOrderMatchResultsResponse, JsonElement>> GetOrderMatchResultsCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawOrdersResponse, JsonElement>> GetOrdersCallAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawMatchResultsResponse, JsonElement>> GetMatchResultsCallAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawDepositWithdrawsResponse, JsonElement>> GetDepositWithdrawsCallAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default);

    Task<BittradeRawCall<RawRetailOrdersResponse, JsonElement>> GetRetailOrdersCallAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default);
}
