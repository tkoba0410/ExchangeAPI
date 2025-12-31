using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
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
        var tick = response.Tick ?? throw new ExchangeApiException("Bittrade ticker response missing tick.");
        return BittradeNormalizer.NormalizeTicker(tick, response.Ts);
    }

    public async Task<BittradeOrderBookNormalized> GetOrderBookAsync(string symbol, CancellationToken ct = default)
    {
        var response = await _raw.GetOrderBookAsync(RawSymbol.From(symbol), cancellationToken: ct).ConfigureAwait(false);
        RequireOk(response.Status, "orderbook");
        var tick = response.Tick ?? throw new ExchangeApiException("Bittrade order book response missing tick.");
        return BittradeNormalizer.NormalizeOrderBook(tick);
    }

    public async Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(string symbol, CancellationToken ct = default)
    {
        var response = await _raw.GetTradesAsync(RawSymbol.From(symbol), ct).ConfigureAwait(false);
        RequireOk(response.Status, "trades");
        var entries = response.Tick?.Data ?? throw new ExchangeApiException("Bittrade trades response missing data.");
        return BittradeNormalizer.NormalizeExecutions(entries);
    }

    private static void RequireOk(string? status, string operation)
    {
        if (!string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException($"Bittrade {operation} response status invalid: {status}.");
        }
    }
}
