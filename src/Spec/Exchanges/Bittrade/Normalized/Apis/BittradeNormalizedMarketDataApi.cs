using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal sealed class BittradeNormalizedMarketDataApi : IBittradeNormalizedMarketDataApi
{
    private readonly IBittradeRawMarketDataApi _raw;

    internal BittradeNormalizedMarketDataApi(IBittradeRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<BittradeTickerNormalized> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        var response = await _raw.GetTickerAsync(RawSymbol.From(symbol), ct).ConfigureAwait(false);
        RequireOk(response.Status, "ticker");
        var tick = response.Tick ?? throw new BittradeNormalizedException("Bittrade ticker response missing tick.");
        return BittradeNormalizer.NormalizeTicker(tick, response.Ts);
    }

    public async Task<BittradeOrderBookNormalized> GetOrderBookAsync(string symbol, CancellationToken ct = default)
    {
        var response = await _raw.GetOrderBookAsync(RawSymbol.From(symbol), cancellationToken: ct).ConfigureAwait(false);
        RequireOk(response.Status, "orderbook");
        var tick = response.Tick ?? throw new BittradeNormalizedException("Bittrade order book response missing tick.");
        return BittradeNormalizer.NormalizeOrderBook(tick);
    }

    public async Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(string symbol, CancellationToken ct = default)
    {
        var response = await _raw.GetTradesAsync(RawSymbol.From(symbol), ct).ConfigureAwait(false);
        RequireOk(response.Status, "trades");
        var entries = response.Tick?.Data ?? throw new BittradeNormalizedException("Bittrade trades response missing data.");
        return BittradeNormalizer.NormalizeExecutions(entries);
    }

    public async Task<BittradeNormalizedCall<BittradeTickerNormalized, JsonElement>> GetTickerCallAsync(
        string symbol,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetTickerCallAsync(RawSymbol.From(symbol), ct).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetTicker", new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
        });

        return CreateCall(
            rawCall,
            request,
            ok =>
            {
                RequireOk(ok.Status, "ticker");
                var tick = ok.Tick ?? throw new BittradeNormalizedException("Bittrade ticker response missing tick.");
                return BittradeNormalizer.NormalizeTicker(tick, ok.Ts);
            });
    }

    public async Task<BittradeNormalizedCall<BittradeOrderBookNormalized, JsonElement>> GetOrderBookCallAsync(
        string symbol,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetOrderBookCallAsync(RawSymbol.From(symbol), cancellationToken: ct).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetOrderBook", new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
        });

        return CreateCall(
            rawCall,
            request,
            ok =>
            {
                RequireOk(ok.Status, "orderbook");
                var tick = ok.Tick ?? throw new BittradeNormalizedException("Bittrade order book response missing tick.");
                return BittradeNormalizer.NormalizeOrderBook(tick);
            });
    }

    public async Task<BittradeNormalizedCall<IReadOnlyList<BittradeExecutionNormalized>, JsonElement>> GetExecutionsCallAsync(
        string symbol,
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetTradesCallAsync(RawSymbol.From(symbol), ct).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetExecutions", new Dictionary<string, string?>
        {
            ["symbol"] = symbol,
        });

        return CreateCall(
            rawCall,
            request,
            ok =>
            {
                RequireOk(ok.Status, "trades");
                var entries = ok.Tick?.Data ?? throw new BittradeNormalizedException("Bittrade trades response missing data.");
                return BittradeNormalizer.NormalizeExecutions(entries);
            });
    }

    private static BittradeNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BittradeNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BittradeRawCall<TRaw, JsonElement> rawCall,
        BittradeNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BittradeNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BittradeNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }

    private static void RequireOk(string? status, string operation)
    {
        if (!string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new BittradeNormalizedException($"Bittrade {operation} response status invalid: {status}.");
        }
    }
}
