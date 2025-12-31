using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Transport;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireMarketDataApi : IBittradeWireMarketDataApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly IRestClient _restClient;

    public BittradeWireMarketDataApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<WireResponse> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = $"market/detail/merged?symbol={ToApiSymbol(symbol)}";
        var meta = await _restClient.GetRawAsync(path, cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetOrderBookAsync(string symbol, string? type = null, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var depthType = string.IsNullOrWhiteSpace(type) ? "step0" : type;
        var path = $"market/depth?symbol={ToApiSymbol(symbol)}&type={depthType}";
        var meta = await _restClient.GetRawAsync(path, cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetTradesAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = $"market/trade?symbol={ToApiSymbol(symbol)}";
        var meta = await _restClient.GetRawAsync(path, cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetKlinesAsync(string symbol, string period, int? size = null, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new ArgumentException("period is required.", nameof(period));
        }

        var sizeParam = size.HasValue ? $"&size={size.Value}" : string.Empty;
        var path = $"market/history/kline?period={period}&symbol={ToApiSymbol(symbol)}{sizeParam}";
        var meta = await _restClient.GetRawAsync(path, cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetTickersAsync(CancellationToken ct = default)
    {
        var meta = await _restClient.GetRawAsync("market/tickers", cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    public async Task<WireResponse> GetTradeHistoryAsync(string symbol, CancellationToken ct = default)
    {
        EnsureSymbol(symbol);
        var path = $"market/history/trade?symbol={ToApiSymbol(symbol)}";
        var meta = await _restClient.GetRawAsync(path, cancellationToken: ct).ConfigureAwait(false);
        return ToWire(meta);
    }

    private static string ToApiSymbol(string symbol) =>
        symbol.Replace("/", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

    private static void EnsureSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }

    private static WireResponse ToWire(HttpResponseMeta meta)
    {
        var headers = meta.Headers is null
            ? null
            : new Dictionary<string, string>(meta.Headers, StringComparer.OrdinalIgnoreCase);
        return new WireResponse(
            Exchange,
            meta.StatusCode,
            meta.Body ?? string.Empty,
            headers);
    }
}
