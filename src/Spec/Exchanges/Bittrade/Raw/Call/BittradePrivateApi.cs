using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;
using ExchangeApi.Spec.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

/// <summary>
/// Bittrade Private REST API（情報系 GET）の Raw 実装。
/// </summary>
internal sealed class BittradePrivateApi : IBittradePrivateApi
{
    private readonly IWireTransport _wire;

    public BittradePrivateApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawAccountsResponse> GetAccountsAsync(CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetAccounts(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawAccountsResponse>(response.Json, "Bittrade.GetAccounts");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetAccounts", response.StatusCode, response.Json);
    }

    public async Task<RawBalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetAccountBalance(accountId), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
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
        var call = await SendAsync(BittradeEndpoints.GetOpenOrders(symbol.Value, accountId), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
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
        var call = await SendAsync(BittradeEndpoints.GetOrder(orderId.Value), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawOrderDetailResponse>(response.Json, "Bittrade.GetOrder");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetOrder", response.StatusCode, response.Json);
    }

    public async Task<RawOrderMatchResultsResponse> GetOrderMatchResultsAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetOrderMatchResults(orderId.Value), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
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
        var call = await SendAsync(
                BittradeEndpoints.GetOrders(symbol.Value, states, startDate, endDate, from, direct, size),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
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
        var call = await SendAsync(
                BittradeEndpoints.GetMatchResults(symbol?.Value, types, startDate, endDate, from, direct, size),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
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
        var call = await SendAsync(
                BittradeEndpoints.GetDepositWithdraws(type, currency, from, size, direct),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
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
        var call = await SendAsync(
                BittradeEndpoints.GetRetailOrders(direct, status, startTime, endTime),
                cancellationToken)
            .ConfigureAwait(false);
        var response = call.Response;
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

    public async Task<BittradeRawCall<RawAccountsResponse, JsonElement>> GetAccountsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BittradeEndpoints.GetAccounts(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetAccounts", new Dictionary<string, string?>());
        return CreateCall<RawAccountsResponse>(request, wireCall, "Bittrade.GetAccounts");
    }

    public async Task<BittradeRawCall<RawBalancesResponse, JsonElement>> GetAccountBalanceCallAsync(
        string accountId,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(accountId, nameof(accountId));
        var wireCall = await SendAsync(BittradeEndpoints.GetAccountBalance(accountId), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetAccountBalance", new Dictionary<string, string?>
        {
            ["accountId"] = accountId,
        });
        return CreateCall<RawBalancesResponse>(request, wireCall, "Bittrade.GetAccountBalance");
    }

    public async Task<BittradeRawCall<RawOpenOrdersResponse, JsonElement>> GetOpenOrdersCallAsync(
        RawSymbol symbol,
        string accountId,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        EnsureRequired(accountId, nameof(accountId));
        var wireCall = await SendAsync(BittradeEndpoints.GetOpenOrders(symbol.Value, accountId), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetOpenOrders", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
            ["accountId"] = accountId,
        });
        return CreateCall<RawOpenOrdersResponse>(request, wireCall, "Bittrade.GetOpenOrders");
    }

    public async Task<BittradeRawCall<RawOrderDetailResponse, JsonElement>> GetOrderCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        var wireCall = await SendAsync(BittradeEndpoints.GetOrder(orderId.Value), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetOrder", new Dictionary<string, string?>
        {
            ["orderId"] = orderId.Value,
        });
        return CreateCall<RawOrderDetailResponse>(request, wireCall, "Bittrade.GetOrder");
    }

    public async Task<BittradeRawCall<RawOrderMatchResultsResponse, JsonElement>> GetOrderMatchResultsCallAsync(
        RawOrderId orderId,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(orderId.Value, nameof(orderId));
        var wireCall = await SendAsync(BittradeEndpoints.GetOrderMatchResults(orderId.Value), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetOrderMatchResults", new Dictionary<string, string?>
        {
            ["orderId"] = orderId.Value,
        });
        return CreateCall<RawOrderMatchResultsResponse>(request, wireCall, "Bittrade.GetOrderMatchResults");
    }

    public async Task<BittradeRawCall<RawOrdersResponse, JsonElement>> GetOrdersCallAsync(
        RawSymbol symbol,
        string states,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        EnsureRequired(states, nameof(states));
        var wireCall = await SendAsync(
                BittradeEndpoints.GetOrders(symbol.Value, states, startDate, endDate, from, direct, size),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetOrders", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
            ["states"] = states,
            ["startDate"] = startDate,
            ["endDate"] = endDate,
            ["from"] = from?.ToString(),
            ["direct"] = direct,
            ["size"] = size?.ToString(),
        });
        return CreateCall<RawOrdersResponse>(request, wireCall, "Bittrade.GetOrders");
    }

    public async Task<BittradeRawCall<RawMatchResultsResponse, JsonElement>> GetMatchResultsCallAsync(
        RawSymbol? symbol = null,
        string? types = null,
        string? startDate = null,
        string? endDate = null,
        long? from = null,
        string? direct = null,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BittradeEndpoints.GetMatchResults(symbol?.Value, types, startDate, endDate, from, direct, size),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetMatchResults", new Dictionary<string, string?>
        {
            ["symbol"] = symbol?.Value,
            ["types"] = types,
            ["startDate"] = startDate,
            ["endDate"] = endDate,
            ["from"] = from?.ToString(),
            ["direct"] = direct,
            ["size"] = size?.ToString(),
        });
        return CreateCall<RawMatchResultsResponse>(request, wireCall, "Bittrade.GetMatchResults");
    }

    public async Task<BittradeRawCall<RawDepositWithdrawsResponse, JsonElement>> GetDepositWithdrawsCallAsync(
        string type,
        string? currency = null,
        long? from = null,
        int? size = null,
        string? direct = null,
        CancellationToken cancellationToken = default)
    {
        EnsureRequired(type, nameof(type));
        var wireCall = await SendAsync(
                BittradeEndpoints.GetDepositWithdraws(type, currency, from, size, direct),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetDepositWithdraws", new Dictionary<string, string?>
        {
            ["type"] = type,
            ["currency"] = currency,
            ["from"] = from?.ToString(),
            ["size"] = size?.ToString(),
            ["direct"] = direct,
        });
        return CreateCall<RawDepositWithdrawsResponse>(request, wireCall, "Bittrade.GetDepositWithdraws");
    }

    public async Task<BittradeRawCall<RawRetailOrdersResponse, JsonElement>> GetRetailOrdersCallAsync(
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(
                BittradeEndpoints.GetRetailOrders(direct, status, startTime, endTime),
                cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetRetailOrders", new Dictionary<string, string?>
        {
            ["direct"] = direct.ToString(),
            ["status"] = status?.ToString(),
            ["startTime"] = startTime?.ToString("O"),
            ["endTime"] = endTime?.ToString("O"),
        });
        return CreateCall<RawRetailOrdersResponse>(request, wireCall, "Bittrade.GetRetailOrders");
    }

    private static BittradeRawRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BittradeRawCall<TOk, JsonElement> CreateCall<TOk>(
        BittradeRawRequest request,
        WireCall call,
        string context)
    {
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            var ok = BittradeRawJson.DeserializeOrThrow<TOk>(response.Json, context);
            return new BittradeRawCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(ok, response.StatusCode),
                call.Meta);
        }

        if (BittradeRawJson.TryDeserialize<JsonElement>(response.Json, out var error, out _))
        {
            return new BittradeRawCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(error!, response.StatusCode),
                call.Meta);
        }

        return new BittradeRawCall<TOk, JsonElement>(
            request,
            new Err<TOk, JsonElement>(default, response.StatusCode),
            call.Meta);
    }

    private static void EnsureRequired(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{name} is required.", name);
        }
    }

    private static void EnsureSymbol(RawSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }

    private Task<WireCall> SendAsync(WireRequest request, CancellationToken ct) =>
        _wire.SendAsync(ExchangeCode.Bittrade, request, ct);
}
