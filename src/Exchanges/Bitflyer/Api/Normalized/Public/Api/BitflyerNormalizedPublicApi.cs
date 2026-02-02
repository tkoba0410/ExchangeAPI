using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Constants;
using ExchangeApi.Primitives.CallCommon;
using RawPublicRequests = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;
using PublicRequests = ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Api;

internal sealed class BitflyerNormalizedPublicApi
{
    private readonly IBitflyerRawApi _raw;

    internal BitflyerNormalizedPublicApi(IBitflyerRawApi raw)
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
            Component(BitflyerEndpointIds.GetMarkets),
            raw => MapResult<IReadOnlyList<BitflyerMarketNormalized>>.Ok(
                raw.Select(BitflyerMarketNormalizer.Normalize).ToArray()));
    }

    public async Task<Call<PublicRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTickerCallAsync(new RawPublicRequests.GetTickerRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetTickerRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetTicker),
            raw => MapResult<BitflyerTickerNormalized>.Ok(
                BitflyerTickerNormalizer.Normalize(raw, rawCall.Meta.RawJson)));
    }

    public async Task<Call<PublicRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardCallAsync(new RawPublicRequests.GetBoardRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetOrderBookRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetBoard),
            raw => MapResult<BitflyerOrderBookNormalized>.Ok(BitflyerOrderBookNormalizer.Normalize(raw)));
    }

    public async Task<Call<PublicRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetExecutionsPublicCallAsync(new RawPublicRequests.GetExecutionsRequest(productCode, count, before, after), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetExecutionsRequest(productCode, count, before, after);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetExecutionsPublic),
            raw =>
            {
                if (!BitflyerExecutionNormalizer.TryNormalizeList(raw, rawCall.Meta.RawJson, out var executions, out var error))
                {
                    return MapResult<IReadOnlyList<BitflyerExecutionNormalized>>.Fail(error!);
                }

                return MapResult<IReadOnlyList<BitflyerExecutionNormalized>>.Ok(executions!);
            });
    }

    public async Task<Call<PublicRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetHealthCallAsync(new RawPublicRequests.GetHealthRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetHealthRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetHealth),
            raw => MapResult<BitflyerHealthNormalized>.Ok(BitflyerHealthNormalizer.Normalize(raw)));
    }

    public async Task<Call<PublicRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardStateCallAsync(new RawPublicRequests.GetBoardStateRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetBoardStateRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetBoardState),
            raw => MapResult<BitflyerBoardStateNormalized>.Ok(BitflyerBoardStateNormalizer.Normalize(raw)));
    }

    public async Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetChatsCallAsync(new RawPublicRequests.GetChatsRequest(fromDate), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetChatsRequest(fromDate);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetChats),
            raw => MapResult<IReadOnlyList<BitflyerChatNormalized>>.Ok(
                raw.Select(BitflyerChatNormalizer.Normalize).ToArray()));
    }

    public async Task<Call<PublicRequests.GetCorporateLeverageRequest, BitflyerCorporateLeverageNormalized>> GetCorporateLeverageCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetCorporateLeverageCallAsync(new RawPublicRequests.GetCorporateLeverageRequest(), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetCorporateLeverageRequest();

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetCorporateLeverage),
            raw => MapResult<BitflyerCorporateLeverageNormalized>.Ok(
                new BitflyerCorporateLeverageNormalized(
                    raw.CurrentMax,
                    raw.CurrentStartDate,
                    raw.NextMax,
                    raw.NextStartDate)));
    }

    public async Task<Call<PublicRequests.GetFundingRateRequest, BitflyerFundingRateNormalized>> GetFundingRateCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetFundingRateCallAsync(new RawPublicRequests.GetFundingRateRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetFundingRateRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetFundingRate),
            raw => MapResult<BitflyerFundingRateNormalized>.Ok(
                new BitflyerFundingRateNormalized(
                    raw.CurrentFundingRate,
                    raw.NextFundingRateSettleDate)));
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

    private static string Component(string endpointId) => $"Bitflyer.{endpointId}";

    private readonly record struct MapResult<TOk>(TOk? Value, CallError? Error)
    {
        public static MapResult<TOk> Ok(TOk value) => new(value, null);
        public static MapResult<TOk> Fail(CallError error) => new(default, error);
    }
}
