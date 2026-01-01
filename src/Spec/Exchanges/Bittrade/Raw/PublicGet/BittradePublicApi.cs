using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Spec.Wire;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw 実装。
/// </summary>
internal sealed class BittradePublicApi : IBittradePublicApi
{
    private readonly IWireTransport _wire;

    public BittradePublicApi(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawMergedResponse> GetMergedTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var call = await SendAsync(BittradeEndpoints.GetTicker(symbol.Value), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawMergedResponse>(response.Json, "Bittrade.GetMergedTicker");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetMergedTicker",
            response.StatusCode,
            response.Json);
    }

    public async Task<RawDepthResponse> GetDepthAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var call = await SendAsync(BittradeEndpoints.GetOrderBook(symbol.Value, type), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawDepthResponse>(response.Json, "Bittrade.GetDepth");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetDepth", response.StatusCode, response.Json);
    }

    public async Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var call = await SendAsync(BittradeEndpoints.GetTrades(symbol.Value), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawTradeResponse>(response.Json, "Bittrade.GetTrades");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetTrades", response.StatusCode, response.Json);
    }

    public async Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetSymbols(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawSymbolsResponse>(response.Json, "Bittrade.GetSymbols");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetSymbols", response.StatusCode, response.Json);
    }

    public async Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetCurrencies(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCurrenciesResponse>(response.Json, "Bittrade.GetCurrencies");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetCurrencies", response.StatusCode, response.Json);
    }

    public async Task<RawTimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetTimestamp(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawTimestampResponse>(response.Json, "Bittrade.GetTimestamp");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetTimestamp", response.StatusCode, response.Json);
    }

    public async Task<RawKlinesResponse> GetKlinesAsync(RawSymbol symbol, string period, int? size = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period)) throw new ArgumentException("period is required.", nameof(period));
        var call = await SendAsync(BittradeEndpoints.GetKlines(symbol.Value, period, size), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawKlinesResponse>(response.Json, "Bittrade.GetKlines");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetKlines", response.StatusCode, response.Json);
    }

    public async Task<RawTickersResponse> GetTickersAsync(CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetTickers(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawTickersResponse>(response.Json, "Bittrade.GetTickers");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetTickers", response.StatusCode, response.Json);
    }

    public async Task<RawTradeHistoryResponse> GetTradeHistoryAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var call = await SendAsync(BittradeEndpoints.GetTradeHistory(symbol.Value), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawTradeHistoryResponse>(
                response.Json,
                "Bittrade.GetTradeHistory");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetTradeHistory",
            response.StatusCode,
            response.Json);
    }

    public async Task<BittradeRawCall<RawMergedResponse, JsonElement>> GetMergedTickerCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var wireCall = await SendAsync(BittradeEndpoints.GetTicker(symbol.Value), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetMergedTicker", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
        });
        return CreateCall<RawMergedResponse>(request, wireCall, "Bittrade.GetMergedTicker");
    }

    public async Task<BittradeRawCall<RawDepthResponse, JsonElement>> GetDepthCallAsync(
        RawSymbol symbol,
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var wireCall = await SendAsync(BittradeEndpoints.GetOrderBook(symbol.Value, type), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetDepth", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
            ["type"] = type,
        });
        return CreateCall<RawDepthResponse>(request, wireCall, "Bittrade.GetDepth");
    }

    public async Task<BittradeRawCall<RawTradeResponse, JsonElement>> GetTradesCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var wireCall = await SendAsync(BittradeEndpoints.GetTrades(symbol.Value), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetTrades", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
        });
        return CreateCall<RawTradeResponse>(request, wireCall, "Bittrade.GetTrades");
    }

    public async Task<BittradeRawCall<RawSymbolsResponse, JsonElement>> GetSymbolsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BittradeEndpoints.GetSymbols(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetSymbols", new Dictionary<string, string?>());
        return CreateCall<RawSymbolsResponse>(request, wireCall, "Bittrade.GetSymbols");
    }

    public async Task<BittradeRawCall<RawCurrenciesResponse, JsonElement>> GetCurrenciesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BittradeEndpoints.GetCurrencies(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetCurrencies", new Dictionary<string, string?>());
        return CreateCall<RawCurrenciesResponse>(request, wireCall, "Bittrade.GetCurrencies");
    }

    public async Task<BittradeRawCall<RawTimestampResponse, JsonElement>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BittradeEndpoints.GetTimestamp(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetTimestamp", new Dictionary<string, string?>());
        return CreateCall<RawTimestampResponse>(request, wireCall, "Bittrade.GetTimestamp");
    }

    public async Task<BittradeRawCall<RawKlinesResponse, JsonElement>> GetKlinesCallAsync(
        RawSymbol symbol,
        string period,
        int? size = null,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new ArgumentException("period is required.", nameof(period));
        }

        var wireCall = await SendAsync(BittradeEndpoints.GetKlines(symbol.Value, period, size), cancellationToken)
            .ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetKlines", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
            ["period"] = period,
            ["size"] = size?.ToString(),
        });
        return CreateCall<RawKlinesResponse>(request, wireCall, "Bittrade.GetKlines");
    }

    public async Task<BittradeRawCall<RawTickersResponse, JsonElement>> GetTickersCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BittradeEndpoints.GetTickers(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetTickers", new Dictionary<string, string?>());
        return CreateCall<RawTickersResponse>(request, wireCall, "Bittrade.GetTickers");
    }

    public async Task<BittradeRawCall<RawTradeHistoryResponse, JsonElement>> GetTradeHistoryCallAsync(
        RawSymbol symbol,
        CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var wireCall = await SendAsync(BittradeEndpoints.GetTradeHistory(symbol.Value), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetTradeHistory", new Dictionary<string, string?>
        {
            ["symbol"] = symbol.Value,
        });
        return CreateCall<RawTradeHistoryResponse>(request, wireCall, "Bittrade.GetTradeHistory");
    }

    public async Task<BittradeRawCall<RawRetailMaintainTimeResponse, JsonElement>> GetRetailMaintainTimeCallAsync(
        CancellationToken cancellationToken = default)
    {
        var wireCall = await SendAsync(BittradeEndpoints.GetRetailMaintainTime(), cancellationToken).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetRetailMaintainTime", new Dictionary<string, string?>());
        return CreateCall<RawRetailMaintainTimeResponse>(request, wireCall, "Bittrade.GetRetailMaintainTime");
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

    public async Task<RawRetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default)
    {
        var call = await SendAsync(BittradeEndpoints.GetRetailMaintainTime(), cancellationToken).ConfigureAwait(false);
        var response = call.Response;
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawRetailMaintainTimeResponse>(
                response.Json,
                "Bittrade.GetRetailMaintainTime");
        }

        throw BittradeRawJson.CreateStatusException(
            "Bittrade.GetRetailMaintainTime",
            response.StatusCode,
            response.Json);
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
