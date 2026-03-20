using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using RawPublicRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Requests;
using PublicRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;

internal sealed class NormalizedPublicApi
{
    private readonly IRawApi _raw;

    internal NormalizedPublicApi(IRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<PublicRequests.GetMarketsRequest, GetMarketsResponse>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetMarketsCallAsync(new RawPublicRequests.GetMarketsRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetMarketsRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetMarkets),
            NoBusinessError,
            raw =>
            {
                var mapped = new List<MarketNormalized>(raw.Count);
                foreach (var entry in raw)
                {
                    if (!MarketNormalizer.TryNormalize(entry, out var market, out var error))
                    {
                        return MapResult<GetMarketsResponse>.Fail(error!);
                    }

                    mapped.Add(market!);
                }

                return MapResult<GetMarketsResponse>.Ok(
                    new GetMarketsResponse(mapped.Select(static x => new GetMarketsItem(x)).ToArray()));
            });
    }

    public async Task<Call<PublicRequests.GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetTickerCallAsync(new RawPublicRequests.GetTickerRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetTickerRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetTicker),
            NoBusinessError,
            raw =>
            {
                if (!TickerNormalizer.TryNormalize(raw, rawCall.Meta.RawJson, out var ticker, out var error))
                {
                    return MapResult<GetTickerResponse>.Fail(error!);
                }

                return MapResult<GetTickerResponse>.Ok(new GetTickerResponse(
                    ticker!.ProductCode,
                    ticker.Timestamp,
                    ticker.TickId,
                    ticker.BestBid,
                    ticker.BestAsk,
                    ticker.BestBidSize,
                    ticker.BestAskSize,
                    ticker.TotalBidDepth,
                    ticker.TotalAskDepth,
                    ticker.LastTradedPrice,
                    ticker.Volume,
                    ticker.VolumeByProduct,
                    ticker.RawSnapshot,
                    ticker.Extras));
            });
    }

    public async Task<Call<PublicRequests.GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBoardCallAsync(new RawPublicRequests.GetBoardRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetBoardRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetBoard),
            NoBusinessError,
            raw =>
            {
                if (!OrderBookNormalizer.TryNormalize(raw, out var orderBook, out var error))
                {
                    return MapResult<GetBoardResponse>.Fail(error!);
                }

                return MapResult<GetBoardResponse>.Ok(new GetBoardResponse(
                    orderBook!.MidPrice,
                    orderBook!.Bids,
                    orderBook.Asks));
            });
    }

    public async Task<Call<PublicRequests.GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        ProductCode productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default)
    {
        RequestCount? requestCount = count.HasValue ? new RequestCount(count.Value) : null;
        RequestBefore? requestBefore = before.HasValue ? new RequestBefore(before.Value) : null;
        RequestAfter? requestAfter = after.HasValue ? new RequestAfter(after.Value) : null;

        var rawCall = await _raw
            .GetExecutionsPublicCallAsync(new RawPublicRequests.GetExecutionsPublicRequest(productCode, count, before, after), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetExecutionsPublicRequest(productCode, requestCount, requestBefore, requestAfter);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetExecutionsPublic),
            NoBusinessError,
            raw =>
            {
                if (!ExecutionNormalizer.TryNormalizeList(raw, rawCall.Meta.RawJson, out var executions, out var error))
                {
                    return MapResult<GetExecutionsPublicResponse>.Fail(error!);
                }

                return MapResult<GetExecutionsPublicResponse>.Ok(
                    new GetExecutionsPublicResponse(executions!.Select(static x => new GetExecutionsPublicItem(x)).ToArray()));
            });
    }

    public async Task<Call<PublicRequests.GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetHealthCallAsync(new RawPublicRequests.GetHealthRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetHealthRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetHealth),
            NoBusinessError,
            raw =>
            {
                if (!HealthNormalizer.TryNormalize(raw, out var normalized, out var error))
                {
                    return MapResult<GetHealthResponse>.Fail(error!);
                }

                return MapResult<GetHealthResponse>.Ok(new GetHealthResponse(normalized!.Status));
            });
    }

    public async Task<Call<PublicRequests.GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetBoardStateCallAsync(new RawPublicRequests.GetBoardStateRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetBoardStateRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetBoardState),
            NoBusinessError,
            raw =>
            {
                if (!BoardStateNormalizer.TryNormalize(raw, out var normalized, out var error))
                {
                    return MapResult<GetBoardStateResponse>.Fail(error!);
                }

                return MapResult<GetBoardStateResponse>.Ok(new GetBoardStateResponse(
                    normalized!.Health,
                    normalized.State,
                    normalized.Data));
            });
    }

    public async Task<Call<PublicRequests.GetChatsRequest, GetChatsResponse>> GetChatsCallAsync(
        FreeText? fromDate = null,
        CancellationToken cancellationToken = default)
    {
        var fromDateText = fromDate?.Value;
        var rawCall = await _raw
            .GetChatsCallAsync(new RawPublicRequests.GetChatsRequest(fromDateText is null ? null : new FreeText(fromDateText)), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetChatsRequest(fromDate);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetChats),
            NoBusinessError,
            raw =>
            {
                var mapped = new List<ChatNormalized>(raw.Count);
                foreach (var entry in raw)
                {
                    if (!ChatNormalizer.TryNormalize(entry, out var chat, out var error))
                    {
                        return MapResult<GetChatsResponse>.Fail(error!);
                    }

                    mapped.Add(chat!);
                }

                return MapResult<GetChatsResponse>.Ok(
                    new GetChatsResponse(mapped.Select(static x => new GetChatsItem(x)).ToArray()));
            });
    }

    public async Task<Call<PublicRequests.GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetCorporateLeverageCallAsync(new RawPublicRequests.GetCorporateLeverageRequest(), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetCorporateLeverageRequest();

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetCorporateLeverage),
            NoBusinessError,
            raw => MapResult<GetCorporateLeverageResponse>.Ok(
                new GetCorporateLeverageResponse(
                    raw.CurrentMax,
                    raw.CurrentStartDate,
                    raw.NextMax,
                    raw.NextStartDate)));
    }

    public async Task<Call<PublicRequests.GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default)
    {
        var rawCall = await _raw
            .GetFundingRateCallAsync(new RawPublicRequests.GetFundingRateRequest(productCode), cancellationToken)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetFundingRateRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            Component(EndpointIds.GetFundingRate),
            NoBusinessError,
            raw => MapResult<GetFundingRateResponse>.Ok(
                new GetFundingRateResponse(
                    raw.CurrentFundingRate,
                    raw.NextFundingRateSettleDate)));
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

    private static string Component(string endpointId) => $"Bitflyer.{endpointId}";

    private static CallError? NoBusinessError<TRaw>(TRaw _) => null;

    private readonly record struct MapResult<TOk>(TOk? Value, CallError? Error)
    {
        public static MapResult<TOk> Ok(TOk value) => new(value, null);
        public static MapResult<TOk> Fail(CallError error) => new(default, error);
    }
}
