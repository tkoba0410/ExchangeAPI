using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using RawPublicDtos = ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;
using RawPublicRequests = ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;

internal sealed class BittradeNormalizedPublicApi
{
    private readonly IBittradeRawApi _raw;

    internal BittradeNormalizedPublicApi(IBittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<NormalizedRequests.GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetSymbolsCallAsync(new RawPublicRequests.GetSymbolsRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetSymbolsRequest();

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetSymbols),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "symbols", out var error))
                {
                    return MapResult<IReadOnlyList<BittradeSymbolNormalized>>.Fail(error);
                }

                if (ok.Data is null)
                {
                    return MapResult<IReadOnlyList<BittradeSymbolNormalized>>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade symbols response invalid."));
                }

                if (!BittradeNormalizer.TryNormalizeSymbols(ok.Data, out var symbols, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeSymbolNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeSymbolNormalized>>.Ok(symbols!);
            });
    }

    public async Task<Call<NormalizedRequests.GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetCurrencysCallAsync(new RawPublicRequests.GetCurrencysRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetCurrencysRequest();

        return CreateCall<RawPublicRequests.GetCurrencysRequest, RawPublicDtos.GetCurrencysResponse, NormalizedRequests.GetCurrencysRequest, IReadOnlyList<CurrencyCode>>(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetCurrencys),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "currencys", out var error))
                {
                    return MapResult<IReadOnlyList<CurrencyCode>>.Fail(error);
                }

                if (ok.Data is null)
                {
                    return MapResult<IReadOnlyList<CurrencyCode>>.Ok(Array.Empty<CurrencyCode>());
                }
                var codes = new List<CurrencyCode>(ok.Data.Count);
                foreach (var code in ok.Data)
                {
                    codes.Add(CurrencyCodeConverter.FromString(code));
                }

                return MapResult<IReadOnlyList<CurrencyCode>>.Ok(codes);
            });
    }

    public async Task<Call<NormalizedRequests.GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTimestampCallAsync(new RawPublicRequests.GetTimestampRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetTimestampRequest();

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetTimestamp),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "timestamp", out var error))
                {
                    return MapResult<DateTimeOffset>.Fail(error);
                }

                return MapResult<DateTimeOffset>.Ok(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetDetailMergedRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetDetailMergedRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetDetailMergedRequest, BittradeTickerNormalized>(
                request,
                Component(BittradeEndpointIds.GetDetailMerged),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDetailMergedCallAsync(new RawPublicRequests.GetDetailMergedRequest(new Symbol(symbolText)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetDetailMerged),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "ticker", out var error))
                {
                    return MapResult<BittradeTickerNormalized>.Fail(error);
                }

                if (!BittradeNormalizer.TryNormalizeTicker(ok, rawCall.Meta.RawJson, out var ticker, out var normalizeError))
                {
                    return MapResult<BittradeTickerNormalized>.Fail(normalizeError!);
                }

                return MapResult<BittradeTickerNormalized>.Ok(ticker!);
            });
    }

    public async Task<Call<NormalizedRequests.GetDepthRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
        ProductCode productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var normalizedDepthType = depthType ?? BittradeDepthType.Step0;
        var request = new NormalizedRequests.GetDepthRequest(productCode, depthType);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetDepthRequest, BittradeOrderBookNormalized>(
                request,
                Component(BittradeEndpointIds.GetDepth),
                error!,
                startedAt);
        }

        if (!TryGetRawDepthType(normalizedDepthType, out var rawDepth, out var depthError))
        {
            return CreateCallError<NormalizedRequests.GetDepthRequest, BittradeOrderBookNormalized>(
                request,
                Component(BittradeEndpointIds.GetDepth),
                depthError!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDepthCallAsync(new RawPublicRequests.GetDepthRequest(
                new Symbol(symbolText),
                rawDepth is null ? null : new FreeText(rawDepth)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetDepth),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "orderbook", out var error))
                {
                    return MapResult<BittradeOrderBookNormalized>.Fail(error);
                }

                if (!BittradeNormalizer.TryNormalizeOrderBook(ok.Tick!, out var orderBook, out var normalizeError))
                {
                    return MapResult<BittradeOrderBookNormalized>.Fail(normalizeError!);
                }

                return MapResult<BittradeOrderBookNormalized>.Ok(orderBook!);
            });
    }

    public async Task<Call<NormalizedRequests.GetTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetTradeRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>(
                request,
                Component(BittradeEndpointIds.GetTrade),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetTradeCallAsync(new RawPublicRequests.GetTradeRequest(new Symbol(symbolText)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetTrade),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "trades", out var error))
                {
                    return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Fail(error);
                }

                var entries = ok.Tick?.Data;
                if (!BittradeNormalizer.TryNormalizeExecutions(entries!, rawCall.Meta.RawJson, out var executions, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Ok(executions!);
            });
    }

    public async Task<Call<NormalizedRequests.GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        int? size = null,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetHistoryKlineRequest(productCode, period, size);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>(
                request,
                Component(BittradeEndpointIds.GetHistoryKline),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetHistoryKlineCallAsync(new RawPublicRequests.GetHistoryKlineRequest(new Symbol(symbolText), period, size), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetHistoryKline),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "klines", out var error))
                {
                    return MapResult<IReadOnlyList<BittradeKlineNormalized>>.Fail(error);
                }

                if (!BittradeNormalizer.TryNormalizeKlines(ok.Data, out var klines, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeKlineNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeKlineNormalized>>.Ok(klines!);
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
            Component(BittradeEndpointIds.GetTickers),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "tickers", out var error))
                {
                    return MapResult<IReadOnlyList<BittradeTickerEntryNormalized>>.Fail(error);
                }

                if (!BittradeNormalizer.TryNormalizeTickers(ok.Data, out var tickers, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeTickerEntryNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeTickerEntryNormalized>>.Ok(tickers!);
            });
    }

    public async Task<Call<NormalizedRequests.GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetHistoryTradeRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>(
                request,
                Component(BittradeEndpointIds.GetHistoryTrade),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetHistoryTradeCallAsync(new RawPublicRequests.GetHistoryTradeRequest(new Symbol(symbolText)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetHistoryTrade),
            ok =>
            {
                if (!TryRequireOk(ok.Status, "history trades", out var error))
                {
                    return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Fail(error);
                }

                if (!BittradeNormalizer.TryNormalizeTradeHistory(ok.Data, out var history, out var normalizeError))
                {
                    return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Fail(normalizeError!);
                }

                return MapResult<IReadOnlyList<BittradeExecutionNormalized>>.Ok(history!);
            });
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, MapResult<TOk>> mapper)
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
        Func<TRaw, MapResult<TOk>> mapper)
    {
        try
        {
            var result = mapper(raw);
            if (result.Error is not null)
            {
                return new Call<TReq, TOk>(
                    Id: CallId.New(),
                    StartedAt: rawCall.StartedAt,
                    Duration: rawCall.Duration,
                    Request: request,
                    Result: new CallResult<TOk>.Err(result.Error),
                    Meta: rawCall.Meta);
            }

            var mapped = result.Value!;
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

    private static bool TryRequireOk(string? status, string operation, out CallError error)
    {
        if (!string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            error = new CallError(CallErrorKind.Semantic, $"Bittrade {operation} response status invalid: {status}.");
            return false;
        }

        error = null!;
        return true;
    }

    private static bool TryGetRawDepthType(BittradeDepthType depthType, out string rawDepth, out CallError? error)
    {
        switch (depthType)
        {
            case BittradeDepthType.Step0:
                rawDepth = "step0";
                error = null;
                return true;
            case BittradeDepthType.Step1:
                rawDepth = "step1";
                error = null;
                return true;
            case BittradeDepthType.Step2:
                rawDepth = "step2";
                error = null;
                return true;
            case BittradeDepthType.Step3:
                rawDepth = "step3";
                error = null;
                return true;
            case BittradeDepthType.Step4:
                rawDepth = "step4";
                error = null;
                return true;
            case BittradeDepthType.Step5:
                rawDepth = "step5";
                error = null;
                return true;
            default:
                rawDepth = string.Empty;
                error = new CallError(CallErrorKind.Semantic, $"Unsupported depth type: {depthType}.");
                return false;
        }
    }

    private readonly record struct MapResult<TOk>(TOk? Value, CallError? Error)
    {
        public static MapResult<TOk> Ok(TOk value) => new(value, null);
        public static MapResult<TOk> Fail(CallError error) => new(default, error);
    }

    private static string Component(string endpointId) => $"Bittrade.{endpointId}";
}
