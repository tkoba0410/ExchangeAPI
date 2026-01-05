using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalize.Types;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw;
using RawRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Call;

internal sealed class BittradeNormalizedMarketDataApi : IBittradeNormalizedMarketDataApi
{
    private readonly IBittradeRawMarketDataApi _raw;

    internal BittradeNormalizedMarketDataApi(IBittradeRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<BittradeTickerNormalized> GetTickerAsync(string symbol, CancellationToken ct = default)
    {
        var call = await GetTickerCallAsync(symbol, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.GetTicker");
    }

    public async Task<BittradeOrderBookNormalized> GetOrderBookAsync(
        string symbol,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var call = await GetOrderBookCallAsync(symbol, depthType, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.GetOrderBook");
    }

    public async Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(string symbol, CancellationToken ct = default)
    {
        var call = await GetExecutionsCallAsync(symbol, ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.GetExecutions");
    }

    public async Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
        string symbol,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTickerAsync(new RawRequests.GetTickerRequest(symbol), ct)
            .ConfigureAwait(false);
        var request = new GetTickerRequest(symbol);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetTicker",
            ok =>
            {
                RequireOk(ok.Status, "ticker");
                var tick = ok.Tick ?? throw new BittradeNormalizedException("Bittrade ticker response missing tick.");
                return BittradeNormalizer.NormalizeTicker(tick, ok.Ts);
            });
    }

    public async Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetOrderBookCallAsync(
        string symbol,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var normalizedDepthType = depthType ?? BittradeDepthType.Step0;
        var rawCall = await _raw
            .GetOrderBookAsync(new RawRequests.GetOrderBookRequest(symbol, ToRawDepthType(normalizedDepthType)), ct)
            .ConfigureAwait(false);
        var request = new GetOrderBookRequest(symbol, depthType);

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
        string symbol,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTradesAsync(new RawRequests.GetMarketTradesRequest(symbol), ct)
            .ConfigureAwait(false);
        var request = new GetExecutionsRequest(symbol);

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetExecutions",
            ok =>
            {
                RequireOk(ok.Status, "trades");
                var entries = ok.Tick?.Data ?? throw new BittradeNormalizedException("Bittrade trades response missing data.");
                return BittradeNormalizer.NormalizeExecutions(entries);
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

    private static TRes Unwrap<TReq, TRes>(Call<TReq, TRes> call, string operation)
    {
        return call.Result switch
        {
            CallResult<TRes>.Ok ok => ok.Response,
            CallResult<TRes>.Err err => throw new ExchangeApiException(
                message: err.Error.Message,
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                statusCode: err.Error.HttpStatus is int status ? (HttpStatusCode?)status : null,
                innerException: err.Error.Exception),
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
