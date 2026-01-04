using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Spec.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Call;

internal sealed class BittradeNormalizedExchangeInfoApi : IBittradeNormalizedExchangeInfoApi
{
    private readonly IBittradeRawApi _raw;

    internal BittradeNormalizedExchangeInfoApi(IBittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<IReadOnlyList<BittradeSymbolNormalized>> GetSymbolsAsync(CancellationToken ct = default)
    {
        var call = await GetSymbolsCallAsync(ct).ConfigureAwait(false);
        return Unwrap(call, "Bittrade.GetSymbols");
    }

    public async Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetSymbolsAsync(new RawRequests.GetRawSymbolsRequest(), ct)
            .ConfigureAwait(false);
        var request = new GetSymbolsRequest();

        return CreateCall(
            rawCall,
            request,
            "Bittrade.GetSymbols",
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    throw new BittradeNormalizedException("Bittrade symbols response invalid.");
                }

                return BittradeNormalizer.NormalizeSymbols(ok.Data);
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
}
