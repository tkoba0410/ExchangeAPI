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
                RequireOk(ok.Status, "symbols");
                if (ok.Data is null) throw new InvalidOperationException("Bittrade symbols response invalid.");
                return BittradeNormalizer.NormalizeSymbols(ok.Data);
            });
    }

    public async Task<Call<NormalizedRequests.GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetCurrencysCallAsync(new RawPublicRequests.GetCurrenciesRequest(), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetCurrencysRequest();

        return CreateCall<RawPublicRequests.GetCurrenciesRequest, RawPublicDtos.RawCurrenciesResponse, NormalizedRequests.GetCurrencysRequest, IReadOnlyList<CurrencyCode>>(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetCurrencys),
            ok =>
            {
                RequireOk(ok.Status, "currencys");
                if (ok.Data is null) return Array.Empty<CurrencyCode>();
                var codes = new List<CurrencyCode>(ok.Data.Count);
                foreach (var code in ok.Data)
                {
                    codes.Add(CurrencyCodeConverter.FromString(code));
                }

                return codes;
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
                RequireOk(ok.Status, "timestamp");
                return ok.Data;
            });
    }

    public async Task<Call<NormalizedRequests.GetTickerRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetTickerRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetTickerRequest, BittradeTickerNormalized>(
                request,
                Component(BittradeEndpointIds.GetDetailMerged),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDetailMergedCallAsync(new RawPublicRequests.GetMergedTickerRequest(symbolText), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetDetailMerged),
            ok =>
            {
                RequireOk(ok.Status, "ticker");
                return BittradeNormalizer.NormalizeTicker(ok, rawCall.Meta.RawJson);
            });
    }

    public async Task<Call<NormalizedRequests.GetOrderBookRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
        ProductCode productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var normalizedDepthType = depthType ?? BittradeDepthType.Step0;
        var request = new NormalizedRequests.GetOrderBookRequest(productCode, depthType);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetOrderBookRequest, BittradeOrderBookNormalized>(
                request,
                Component(BittradeEndpointIds.GetDepth),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetDepthCallAsync(new RawPublicRequests.GetDepthRequest(symbolText, ToRawDepthType(normalizedDepthType)), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetDepth),
            ok =>
            {
                RequireOk(ok.Status, "orderbook");
                var tick = ok.Tick ?? throw new InvalidOperationException("Bittrade order book response missing tick.");
                return BittradeNormalizer.NormalizeOrderBook(tick);
            });
    }

    public async Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var request = new NormalizedRequests.GetExecutionsRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode.Value, out var symbolText, out var error))
        {
            return CreateCallError<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
                request,
                Component(BittradeEndpointIds.GetTrade),
                error!,
                startedAt);
        }

        var rawCall = await _raw
            .GetTradeCallAsync(new RawPublicRequests.GetTradesRequest(symbolText), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetTrade),
            ok =>
            {
                RequireOk(ok.Status, "trades");
                var entries = ok.Tick?.Data ?? throw new InvalidOperationException("Bittrade trades response missing data.");
                return BittradeNormalizer.NormalizeExecutions(entries, rawCall.Meta.RawJson);
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
            .GetHistoryKlineCallAsync(new RawPublicRequests.GetKlinesRequest(symbolText, period.Value, size), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetHistoryKline),
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
            Component(BittradeEndpointIds.GetTickers),
            ok =>
            {
                RequireOk(ok.Status, "tickers");
                return BittradeNormalizer.NormalizeTickers(ok.Data);
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
            .GetHistoryTradeCallAsync(new RawPublicRequests.GetTradeHistoryRequest(symbolText), ct)
            .ConfigureAwait(false);

        return CreateCall(
            rawCall,
            request,
            Component(BittradeEndpointIds.GetHistoryTrade),
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
            throw new InvalidOperationException($"Bittrade {operation} response status invalid: {status}.");
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
            _ => throw new InvalidOperationException($"Unsupported depth type: {depthType}."),
        };

    private static string Component(string endpointId) => $"Bittrade.{endpointId}";
}
