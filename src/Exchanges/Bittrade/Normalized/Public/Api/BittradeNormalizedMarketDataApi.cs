using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using RawPublicDtos = ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using RawPublicRequests = ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;

internal sealed class BittradeNormalizedMarketDataApi : IBittradeNormalizedMarketDataApi
{
    private readonly IBittradeRawApi _raw;

    internal BittradeNormalizedMarketDataApi(IBittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<NormalizedRequests.GetTickerRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetTickerRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetTickerRequest, BittradeTickerNormalized>(
                request,
                "Bittrade.GetTicker",
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDetailMergedCallAsync(new RawPublicRequests.GetMergedTickerRequest(symbolText), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetTicker",
            ok =>
            {
                RequireOk(ok.Status, "ticker");
                return BittradeNormalizer.NormalizeTicker(ok, rawCall.Meta.RawJson);
            });
    }

    public async Task<Call<NormalizedRequests.GetOrderBookRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
        string productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var normalizedDepthType = depthType ?? BittradeDepthType.Step0;
        var request = new NormalizedRequests.GetOrderBookRequest(productCode, depthType);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetOrderBookRequest, BittradeOrderBookNormalized>(
                request,
                "Bittrade.GetOrderBook",
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDepthCallAsync(new RawPublicRequests.GetDepthRequest(symbolText, ToRawDepthType(normalizedDepthType)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetOrderBook",
            ok =>
            {
                RequireOk(ok.Status, "orderbook");
                var tick = ok.Tick ?? throw new BittradeNormalizedException("Bittrade order book response missing tick.");
                return BittradeNormalizer.NormalizeOrderBook(tick);
            });
    }

    public async Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetExecutionsRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
                request,
                "Bittrade.GetExecutions",
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetTradeCallAsync(new RawPublicRequests.GetTradesRequest(symbolText), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetExecutions",
            ok =>
            {
                RequireOk(ok.Status, "trades");
                var entries = ok.Tick?.Data ?? throw new BittradeNormalizedException("Bittrade trades response missing data.");
                return BittradeNormalizer.NormalizeExecutions(entries, rawCall.Meta.RawJson);
            });
    }

    public async Task<Call<NormalizedRequests.GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>> GetHistoryKlineCallAsync(
        string productCode,
        string period,
        int? size = null,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetHistoryKlineRequest(productCode, period, size);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>(
                request,
                "Bittrade.GetHistoryKline",
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetHistoryKlineCallAsync(new RawPublicRequests.GetKlinesRequest(symbolText, period, size), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetHistoryKline",
            ok =>
            {
                RequireOk(ok.Status, "klines");
                return BittradeNormalizer.NormalizeKlines(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetTickersRequest, IReadOnlyList<BittradeTickerEntryNormalized>>> GetTickersCallAsync(
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetTickersRequest();
        var rawCall = await _raw.GetTickersCallAsync(new RawPublicRequests.GetTickersRequest(), ct).ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetTickers",
            ok =>
            {
                RequireOk(ok.Status, "tickers");
                return BittradeNormalizer.NormalizeTickers(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetHistoryTradeCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetHistoryTradeRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>(
                request,
                "Bittrade.GetHistoryTrade",
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetHistoryTradeCallAsync(new RawPublicRequests.GetTradeHistoryRequest(symbolText), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetHistoryTrade",
            ok =>
            {
                RequireOk(ok.Status, "history trades");
                return BittradeNormalizer.NormalizeTradeHistory(ok.Data);
            });
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: rawCall.Meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: rawCall.Meta)
        };
    }

    private static bool TryGetApiSymbol(string? productCode, out string apiSymbol, out CallError? error)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            apiSymbol = string.Empty;
            error = new CallError(CallErrorKind.Unknown, "ProductCode is required.");
            return false;
        }

        if (!BittradeSymbol.TryParse(productCode, out var symbol))
        {
            apiSymbol = string.Empty;
            error = new CallError(CallErrorKind.Semantic, $"Invalid product code: {productCode}.");
            return false;
        }

        apiSymbol = symbol.Value;
        error = null;
        return true;
    }

    private static Call<TReq, TOk> CreateCallError<TReq, TOk>(
        TReq request,
        string component,
        CallError error,
        DateTimeOffset startedAt)
    {
        var meta = CallMeta.CreateInternal("Normalized", component);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: DateTimeOffset.UtcNow - startedAt,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }

    private static Call<TReq, TOk> MapOk<TRawReq, TReq, TRaw, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        TRaw raw,
        Func<TRaw, TOk> mapper)
    {
        try
        {
            var mapped = mapper(raw);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Ok(mapped),
                Meta: rawCall.Meta);
        }
        catch (Exception ex)
        {
            var error = new CallError(CallErrorKind.Mapping, $"{component} failed to map normalized response.", ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: rawCall.Meta);
        }
    }

    private static void RequireOk(string? status, string operation)
    {
        if (!string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            throw new BittradeNormalizedException($"Bittrade {operation} response status invalid: {status}.");
        }
    }

    private static string ToRawDepthType(BittradeDepthType depthType) =>
        depthType switch
        {
            BittradeDepthType.Step0 => "step0",
            BittradeDepthType.Step1 => "step1",
            BittradeDepthType.Step2 => "step2",
            BittradeDepthType.Step3 => "step3",
            BittradeDepthType.Step4 => "step4",
            BittradeDepthType.Step5 => "step5",
            _ => throw new BittradeNormalizedException($"Unsupported depth type: {depthType}."),
        };
}
