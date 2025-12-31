using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateApi : IBittradePrivateApi
{
    private readonly IBittradeWireAccountApi _wire;

    public BittradePrivateApi(IBittradeWireAccountApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetAccountsAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawAccountsResponse>(response.Json, "Bittrade.GetAccounts");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetAccounts", response.StatusCode, response.Json);
    }

    public async Task<RawBalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetAccountBalanceAsync(accountId, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawBalancesResponse>(
                response.Json,
                "Bittrade.GetAccountBalance");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetAccountBalance",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetOpenOrdersAsync(symbol, accountId, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawOpenOrdersResponse>(
                response.Json,
                "Bittrade.GetOpenOrders");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetOpenOrders",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetOrderAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawOrderDetailResponse>(response.Json, "Bittrade.GetOrder");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetOrder", response.StatusCode, response.Json);
    }

    public async Task<RawOrderMatchResultsResponse> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        var response = await _wire.GetOrderMatchResultsAsync(orderId, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawOrderMatchResultsResponse>(
                response.Json,
                "Bittrade.GetOrderMatchResults");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetOrderMatchResults",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawOrdersResponse> GetOrdersAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetOrdersAsync(symbol, states, startDate, endDate, from, direct, size, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawOrdersResponse>(response.Json, "Bittrade.GetOrders");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetOrders", response.StatusCode, response.Json);
    }

    public async Task<RawMatchResultsResponse> GetMatchResultsAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetMatchResultsAsync(symbol, types, startDate, endDate, from, direct, size, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawMatchResultsResponse>(
                response.Json,
                "Bittrade.GetMatchResults");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetMatchResults",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawDepositWithdrawsResponse> GetDepositWithdrawsAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetDepositWithdrawsAsync(type, currency, from, size, direct, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawDepositWithdrawsResponse>(
                response.Json,
                "Bittrade.GetDepositWithdraws");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetDepositWithdraws",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawRetailOrdersResponse> GetRetailOrdersAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var response = await _wire
            .GetRetailOrdersAsync(direct, status, startTime, endTime, cancellationToken)
            .ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawRetailOrdersResponse>(
                response.Json,
                "Bittrade.GetRetailOrders");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetRetailOrders",
            response.StatusCode,
            response.Json);
    }
}
