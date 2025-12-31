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
        var response = await _wire.MarketData.GetOrderBookAsync(symbol.Value, type, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawDepthResponse>(response.Json, "Bittrade.GetDepth");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetDepth", response.StatusCode, response.Json);
    }

    public async Task<RawTradeResponse> GetTradesAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var response = await _wire.MarketData.GetTradesAsync(symbol.Value, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawTradeResponse>(response.Json, "Bittrade.GetTrades");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetTrades", response.StatusCode, response.Json);
    }

    public async Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetSymbolsAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawSymbolsResponse>(response.Json, "Bittrade.GetSymbols");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetSymbols", response.StatusCode, response.Json);
    }

    public async Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetCurrenciesAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawCurrenciesResponse>(response.Json, "Bittrade.GetCurrencies");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetCurrencies", response.StatusCode, response.Json);
    }

    public async Task<RawTimestampResponse> GetTimestampAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetTimestampAsync(cancellationToken).ConfigureAwait(false);
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
        var response = await _wire.MarketData.GetKlinesAsync(symbol.Value, period, size, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawKlinesResponse>(response.Json, "Bittrade.GetKlines");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetKlines", response.StatusCode, response.Json);
    }

    public async Task<RawTickersResponse> GetTickersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.MarketData.GetTickersAsync(cancellationToken).ConfigureAwait(false);
        if (response.StatusCode is >= 200 and < 300)
        {
            return BittradeRawJson.DeserializeOrThrow<RawTickersResponse>(response.Json, "Bittrade.GetTickers");
        }

        throw BittradeRawJson.CreateStatusException("Bittrade.GetTickers", response.StatusCode, response.Json);
    }

    public async Task<RawTradeHistoryResponse> GetTradeHistoryAsync(RawSymbol symbol, CancellationToken cancellationToken = default)
    {
        EnsureSymbol(symbol);
        var response = await _wire.MarketData.GetTradeHistoryAsync(symbol.Value, cancellationToken).ConfigureAwait(false);
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

    public async Task<RawRetailMaintainTimeResponse> GetRetailMaintainTimeAsync(CancellationToken cancellationToken = default)
    {
        var response = await _wire.Common.GetRetailMaintainTimeAsync(cancellationToken).ConfigureAwait(false);
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
}
