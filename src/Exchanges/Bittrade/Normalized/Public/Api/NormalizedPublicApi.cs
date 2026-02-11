using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Constants;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using RawPublicDtos = ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using RawPublicRequests = ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;

internal sealed class NormalizedPublicApi
{
    private readonly IBittradeRawApi _raw;

    internal NormalizedPublicApi(IBittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<NormalizedRequests.GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetSymbolsCallAsync(new RawPublicRequests.GetSymbolsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetSymbolsRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetSymbols),
            ok => DetectInvalidStatus(ok.Status, "symbols"),
            ok =>
            {
                if (ok.Data is null)
                {
                    return MapResult<GetSymbolsResponse>.Fail(
                        new CallError(CallErrorKind.Mapping, "Bittrade symbols response invalid."));
                }

                if (!Normalizer.TryNormalizeSymbols(ok.Data, out var symbols, out var normalizeError))
                {
                    return MapResult<GetSymbolsResponse>.Fail(normalizeError!);
                }

                return MapResult<GetSymbolsResponse>.Ok(new GetSymbolsResponse(symbols!));
            });
    }

    public async Task<Call<NormalizedRequests.GetCurrenciesRequest, GetCurrenciesResponse>> GetCurrenciesCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCurrenciesCallAsync(new RawPublicRequests.GetCurrenciesRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetCurrenciesRequest();

        return CreateCall<RawPublicRequests.GetCurrenciesRequest, RawPublicDtos.GetCurrenciesResponse, NormalizedRequests.GetCurrenciesRequest, GetCurrenciesResponse>(
            rawCall,
            request,
            Component(EndpointIds.GetCurrencies),
            ok => DetectInvalidStatus(ok.Status, "currencys"),
            ok =>
            {
                if (ok.Data is null)
                {
                    return MapResult<GetCurrenciesResponse>.Ok(new GetCurrenciesResponse(Array.Empty<CurrencyCode>()));
                }
                var codes = new List<CurrencyCode>(ok.Data.Count);
                foreach (var code in ok.Data)
                {
                    codes.Add(CurrencyCodeConverter.FromString(code));
                }

                return MapResult<GetCurrenciesResponse>.Ok(new GetCurrenciesResponse(codes));
            });
    }

    public async Task<Call<NormalizedRequests.GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetTimestampCallAsync(new RawPublicRequests.GetTimestampRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetTimestampRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetTimestamp),
            ok => DetectInvalidStatus(ok.Status, "timestamp"),
            ok =>
            {
                return MapResult<GetTimestampResponse>.Ok(new GetTimestampResponse(ok.Data));
            });
    }

    public async Task<Call<NormalizedRequests.GetDetailMergedRequest, GetDetailMergedResponse>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var request = new NormalizedRequests.GetDetailMergedRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetDetailMergedRequest, GetDetailMergedResponse>(
                request,
                Component(EndpointIds.GetDetailMerged),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDetailMergedCallAsync(new RawPublicRequests.GetDetailMergedRequest(new Symbol(symbolText)), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetDetailMerged),
            ok => DetectInvalidStatus(ok.Status, "ticker"),
            ok =>
            {
                if (!Normalizer.TryNormalizeTicker(ok, rawCall.Meta.RawJson, out var ticker, out var normalizeError))
                {
                    return MapResult<GetDetailMergedResponse>.Fail(normalizeError!);
                }

                return MapResult<GetDetailMergedResponse>.Ok(new GetDetailMergedResponse(
                    ticker!.LastTradedPrice,
                    ticker.Timestamp,
                    ticker.RawSnapshot,
                    ticker.Extras));
            });
    }

    public async Task<Call<NormalizedRequests.GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        ProductCode productCode,
        string? depthType = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedDepthType = depthType ?? "step0";
        var request = new NormalizedRequests.GetDepthRequest(productCode, depthType);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetDepthRequest, GetDepthResponse>(
                request,
                Component(EndpointIds.GetDepth),
                error!,
                startedAt);
        }

        if (!TryGetRawDepthType(normalizedDepthType, out var rawDepth, out var depthError))
        {
            return CreateCallError<NormalizedRequests.GetDepthRequest, GetDepthResponse>(
                request,
                Component(EndpointIds.GetDepth),
                depthError!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDepthCallAsync(new RawPublicRequests.GetDepthRequest(
                new Symbol(symbolText),
                rawDepth is null ? null : new FreeText(rawDepth)), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetDepth),
            ok => DetectInvalidStatus(ok.Status, "orderbook"),
            ok =>
            {
                if (!Normalizer.TryNormalizeOrderBook(ok.Tick!, out var orderBook, out var normalizeError))
                {
                    return MapResult<GetDepthResponse>.Fail(normalizeError!);
                }

                return MapResult<GetDepthResponse>.Ok(new GetDepthResponse(
                    orderBook!.Bids,
                    orderBook.Asks));
            });
    }

    public async Task<Call<NormalizedRequests.GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var request = new NormalizedRequests.GetTradeRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetTradeRequest, GetTradeResponse>(
                request,
                Component(EndpointIds.GetTrade),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetTradeCallAsync(new RawPublicRequests.GetTradeRequest(new Symbol(symbolText)), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetTrade),
            ok => DetectInvalidStatus(ok.Status, "trades"),
            ok =>
            {
                var entries = ok.Tick?.Data;
                if (!Normalizer.TryNormalizeExecutions(entries!, rawCall.Meta.RawJson, out var executions, out var normalizeError))
                {
                    return MapResult<GetTradeResponse>.Fail(normalizeError!);
                }

                return MapResult<GetTradeResponse>.Ok(new GetTradeResponse(executions!));
            });
    }

    public async Task<Call<NormalizedRequests.GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        RequestSize? size = null,
        CancellationToken cancellationToken = default)
    {
        var request = new NormalizedRequests.GetHistoryKlineRequest(productCode, period, size);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetHistoryKlineRequest, GetHistoryKlineResponse>(
                request,
                Component(EndpointIds.GetHistoryKline),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetHistoryKlineCallAsync(
                new RawPublicRequests.GetHistoryKlineRequest(new Symbol(symbolText), period, size?.Value),
                cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetHistoryKline),
            ok => DetectInvalidStatus(ok.Status, "klines"),
            ok =>
            {
                if (!Normalizer.TryNormalizeKlines(ok.Data, out var klines, out var normalizeError))
                {
                    return MapResult<GetHistoryKlineResponse>.Fail(normalizeError!);
                }

                return MapResult<GetHistoryKlineResponse>.Ok(new GetHistoryKlineResponse(klines!));
            });
    }

    public async Task<Call<NormalizedRequests.GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new NormalizedRequests.GetTickersRequest();
        var rawCall = await _raw.GetTickersCallAsync(new RawPublicRequests.GetTickersRequest(), cancellationToken).ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetTickers),
            ok => DetectInvalidStatus(ok.Status, "tickers"),
            ok =>
            {
                if (!Normalizer.TryNormalizeTickers(ok.Data, out var tickers, out var normalizeError))
                {
                    return MapResult<GetTickersResponse>.Fail(normalizeError!);
                }

                return MapResult<GetTickersResponse>.Ok(new GetTickersResponse(tickers!));
            });
    }

    public async Task<Call<NormalizedRequests.GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var request = new NormalizedRequests.GetHistoryTradeRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetHistoryTradeRequest, GetHistoryTradeResponse>(
                request,
                Component(EndpointIds.GetHistoryTrade),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetHistoryTradeCallAsync(new RawPublicRequests.GetHistoryTradeRequest(new Symbol(symbolText)), cancellationToken)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetHistoryTrade),
            ok => DetectInvalidStatus(ok.Status, "history trades"),
            ok =>
            {
                if (!Normalizer.TryNormalizeTradeHistory(ok.Data, out var history, out var normalizeError))
                {
                    return MapResult<GetHistoryTradeResponse>.Fail(normalizeError!);
                }

                return MapResult<GetHistoryTradeResponse>.Ok(new GetHistoryTradeResponse(history!));
            });
    }

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, CallError?> businessErrorDetector,
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
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, businessErrorDetector, mapper),
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

        if (!ExchangeSymbol.TryParse(productCode, out var symbol))
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
        Func<TRaw, CallError?> businessErrorDetector,
        Func<TRaw, MapResult<TOk>> mapper)
    {
        try
        {
            var businessError = businessErrorDetector(raw);
            if (businessError is not null)
            {
                return new Call<TReq, TOk>(
                    Id: CallId.New(),
                    StartedAt: rawCall.StartedAt,
                    Duration: rawCall.Duration,
                    Request: request,
                    Result: new CallResult<TOk>.Err(businessError),
                    Meta: rawCall.Meta);
            }

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

    private static CallError? DetectInvalidStatus(string? status, string operation)
    {
        if (!string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase))
        {
            return new CallError(CallErrorKind.Semantic, $"Bittrade {operation} response status invalid: {status}.");
        }

        return null;
    }

    private static bool TryGetRawDepthType(string? depthType, out string rawDepth, out CallError? error)
    {
        switch (depthType?.Trim().ToLowerInvariant())
        {
            case "step0":
                rawDepth = "step0";
                error = null;
                return true;
            case "step1":
                rawDepth = "step1";
                error = null;
                return true;
            case "step2":
                rawDepth = "step2";
                error = null;
                return true;
            case "step3":
                rawDepth = "step3";
                error = null;
                return true;
            case "step4":
                rawDepth = "step4";
                error = null;
                return true;
            case "step5":
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
