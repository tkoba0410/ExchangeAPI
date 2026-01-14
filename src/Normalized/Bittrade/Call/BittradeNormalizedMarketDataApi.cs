using System;
using System.Collections.Generic;
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

    public async Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
        BittradeSymbol symbol,
        CancellationToken ct = default)
    {
        var symbolText = symbol.ToString();
        var rawCall = await _raw
            .GetTickerAsync(new RawRequests.GetTickerRequest(symbolText), ct)
            .ConfigureAwait(false);
        var request = new GetTickerRequest(symbol);

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
        BittradeSymbol symbol,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default)
    {
        var normalizedDepthType = depthType ?? BittradeDepthType.Step0;
        var symbolText = symbol.ToString();
        var rawCall = await _raw
            .GetOrderBookAsync(new RawRequests.GetOrderBookRequest(symbolText, ToRawDepthType(normalizedDepthType)), ct)
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
        BittradeSymbol symbol,
        CancellationToken ct = default)
    {
        var symbolText = symbol.ToString();
        var rawCall = await _raw
            .GetTradesAsync(new RawRequests.GetMarketTradesRequest(symbolText), ct)
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
