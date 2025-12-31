using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Public REST API の Raw 実装。
/// </summary>
internal sealed class BittradePublicApi : IBittradePublicApi
{
    private readonly IBittradeWireApi _wire;

    public BittradePublicApi(IBittradeWireApi wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public async Task<RawMergedResponse> GetMergedTickerAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var response = await _wire.MarketData.GetTickerAsync(symbol.Value, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawMergedResponse>(response);
    }

    public async Task<RawDepthResponse> GetDepthAsync(RawSymbol symbol, string? type = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var response = await _wire.MarketData.GetOrderBookAsync(symbol.Value, type, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawDepthResponse>(response);
    }

    public async Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var response = await _wire.MarketData.GetTradesAsync(symbol.Value, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawTradeResponse>(response);
    }

    public async Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawSymbolsResponse>(response);
    }

    public async Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetCurrenciesAsync(cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawCurrenciesResponse>(response);
    }

    public async Task<RawTimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetTimestampAsync(cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawTimestampResponse>(response);
    }

    public async Task<RawKlinesResponse> GetKlinesAsync(RawSymbol symbol, string period, int? size = null, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        if (string.IsNullOrWhiteSpace(period)) throw new ArgumentException("period is required.", nameof(period));
        var response = await _wire.MarketData.GetKlinesAsync(symbol.Value, period, size, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawKlinesResponse>(response);
    }

    public async Task<RawTickersResponse> GetTickersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.MarketData.GetTickersAsync(cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawTickersResponse>(response);
    }

    public async Task<RawTradeHistoryResponse> GetTradeHistoryAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var response = await _wire.MarketData.GetTradeHistoryAsync(symbol.Value, cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawTradeHistoryResponse>(response);
    }

    public async Task<RawRetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetRetailMaintainTimeAsync(cancellationToken).ConfigureAwait(false);
        return BittradeRawJson.ParseOrThrow<RawRetailMaintainTimeResponse>(response);
    }

    private static void EnsureSymbol(RawSymbol symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol.Value))
        {
            throw new ArgumentException("symbol is required.", nameof(symbol));
        }
    }
}
