using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using RawPublicRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Requests;
using PublicRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;

internal sealed class BitflyerNormalizedExchangeInfoApi
{
    private readonly IBitflyerRawApi _raw;

    internal BitflyerNormalizedExchangeInfoApi(IBitflyerRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<PublicRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetMarketsCallAsync(new RawPublicRequests.GetMarketsRequest(), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetMarketsRequest();

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetMarkets",
            raw => (IReadOnlyList<BitflyerMarketNormalized>)raw
                .Select(BitflyerMarketNormalizer.Normalize)
                .ToArray());
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
}
