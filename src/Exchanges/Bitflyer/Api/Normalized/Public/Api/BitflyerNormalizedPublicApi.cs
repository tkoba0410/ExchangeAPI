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
using ExchangeApi.Primitives.DomainCommon.Types;
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
            raw =>
            {
                var mapped = new List<BitflyerMarketNormalized>(raw.Count);
                foreach (var entry in raw)
                {
                    if (!BitflyerMarketNormalizer.TryNormalize(entry, out var market, out var error))
                    {
                        return MapResult<IReadOnlyList<BitflyerMarketNormalized>>.Fail(error!);
                    }

                    mapped.Add(market!);
                }

                return MapResult<IReadOnlyList<BitflyerMarketNormalized>>.Ok(mapped.ToArray());
            });
    }

    public async Task<Call<PublicRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTickerCallAsync(new RawPublicRequests.GetTickerRequest(productCode.Value), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetTickerRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetTicker),
            raw =>
            {
                if (!BitflyerTickerNormalizer.TryNormalize(raw, rawCall.Meta.RawJson, out var ticker, out var error))
                {
                    return MapResult<BitflyerTickerNormalized>.Fail(error!);
                }

                return MapResult<BitflyerTickerNormalized>.Ok(ticker!);
            });
    }

    public async Task<Call<PublicRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardCallAsync(new RawPublicRequests.GetBoardRequest(productCode.Value), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetOrderBookRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetBoard),
            raw =>
            {
                if (!BitflyerOrderBookNormalizer.TryNormalize(raw, out var orderBook, out var error))
                {
                    return MapResult<BitflyerOrderBookNormalized>.Fail(error!);
                }

                return MapResult<BitflyerOrderBookNormalized>.Ok(orderBook!);
            });
    }

    public async Task<Call<PublicRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetExecutionsPublicCallAsync(new RawPublicRequests.GetExecutionsRequest(productCode.Value, count, before, after), ct)
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
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetHealthCallAsync(new RawPublicRequests.GetHealthRequest(productCode.Value), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetHealthRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetHealth),
            raw =>
            {
                if (!BitflyerHealthNormalizer.TryNormalize(raw, out var normalized, out var error))
                {
                    return MapResult<BitflyerHealthNormalized>.Fail(error!);
                }

                return MapResult<BitflyerHealthNormalized>.Ok(normalized!);
            });
    }

    public async Task<Call<PublicRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardStateCallAsync(new RawPublicRequests.GetBoardStateRequest(productCode.Value), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetBoardStateRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetBoardState),
            raw =>
            {
                if (!BitflyerBoardStateNormalizer.TryNormalize(raw, out var normalized, out var error))
                {
                    return MapResult<BitflyerBoardStateNormalized>.Fail(error!);
                }

                return MapResult<BitflyerBoardStateNormalized>.Ok(normalized!);
            });
    }

    public async Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken ct = default)
    {
        var fromDateText = fromDate?.Value;
        var rawCall = await _raw
            .GetChatsCallAsync(new RawPublicRequests.GetChatsRequest(fromDateText), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetChatsRequest(fromDate);

        return CreateCall(
            rawCall,
            request,
            Component(BitflyerEndpointIds.GetChats),
            raw =>
            {
                var mapped = new List<BitflyerChatNormalized>(raw.Count);
                foreach (var entry in raw)
                {
                    if (!BitflyerChatNormalizer.TryNormalize(entry, out var chat, out var error))
                    {
                        return MapResult<IReadOnlyList<BitflyerChatNormalized>>.Fail(error!);
                    }

                    mapped.Add(chat!);
                }

                return MapResult<IReadOnlyList<BitflyerChatNormalized>>.Ok(mapped.ToArray());
            });
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
        ProductCode productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetFundingRateCallAsync(new RawPublicRequests.GetFundingRateRequest(productCode.Value), ct)
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
