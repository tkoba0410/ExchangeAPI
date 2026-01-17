using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalized.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using RawRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Call;

internal sealed class BittradeNormalizedMarketDataApi : IBittradeNormalizedMarketDataApi
{
    private readonly IBittradeRawMarketDataApi _raw;

    internal BittradeNormalizedMarketDataApi(IBittradeRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var request = new GetTickerRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<GetTickerRequest, BittradeTickerNormalized>(request, "Bittrade.GetTicker", error!, startedAt);
        }

        var rawCall = await _raw
            .GetTickerAsync(new RawRequests.GetTickerRequest(symbolText), ct)
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

    public async Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetOrderBookCallAsync(
        string productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var normalizedDepthType = depthType ?? BittradeDepthType.Step0;
        var request = new GetOrderBookRequest(productCode, depthType);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<GetOrderBookRequest, BittradeOrderBookNormalized>(request, "Bittrade.GetOrderBook", error!, startedAt);
        }

        var rawCall = await _raw
            .GetOrderBookAsync(new RawRequests.GetOrderBookRequest(symbolText, ToRawDepthType(normalizedDepthType)), ct)
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

    public async Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var request = new GetExecutionsRequest(productCode);
        var startedAt = DateTimeOffset.UtcNow;
        if (!TryGetApiSymbol(productCode, out var symbolText, out var error))
        {
            return CreateCallError<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(request, "Bittrade.GetExecutions", error!, startedAt);
        }

        var rawCall = await _raw
            .GetTradesAsync(new RawRequests.GetMarketTradesRequest(symbolText), ct)
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

    private static Call<TReq, TOk> CreateCall<TRawReq, TRaw, TReq, TOk>(
        Call<TRawReq, TRaw> rawCall,
        TReq request,
        string component,
        Func<TRaw, TOk> mapper)
    {
        var meta = new CallMeta(
            Layer: "Normalized",
            Component: component,
            Tags: null,
            Children: new[] { rawCall.Id });

        return rawCall.Result switch
        {
            CallResult<TRaw>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: meta),
            CallResult<TRaw>.Ok ok => MapOk(rawCall, request, component, ok.Response, mapper, meta),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: rawCall.StartedAt,
                Duration: rawCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Raw call returned unknown result.")),
                Meta: meta)
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
        var meta = new CallMeta(
            Layer: "Normalized",
            Component: component,
            Tags: null,
            Children: null);

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
        Func<TRaw, TOk> mapper,
        CallMeta meta)
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
                Meta: meta);
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
                Meta: meta);
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
