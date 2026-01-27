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

internal sealed class BitflyerNormalizedMarketDataApi
{
    private readonly IBitflyerRawApi _raw;

    internal BitflyerNormalizedMarketDataApi(IBitflyerRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
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
            "Bitflyer.GetTicker",
            raw => BitflyerTickerNormalizer.Normalize(raw, rawCall.Meta.RawJson));
    }

    public async Task<Call<PublicRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardCallAsync(new RawPublicRequests.GetBoardRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetOrderBookRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetBoard", BitflyerOrderBookNormalizer.Normalize);
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
            "Bitflyer.GetExecutions",
            raw => (IReadOnlyList<BitflyerExecutionNormalized>)BitflyerExecutionNormalizer.NormalizeList(
                raw,
                rawCall.Meta.RawJson));
    }

    public async Task<Call<PublicRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetHealthCallAsync(new RawPublicRequests.GetHealthRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetHealthRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetHealth", BitflyerHealthNormalizer.Normalize);
    }

    public async Task<Call<PublicRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardStateCallAsync(new RawPublicRequests.GetBoardStateRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetBoardStateRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetBoardState", BitflyerBoardStateNormalizer.Normalize);
    }

    public async Task<Call<PublicRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetChatsCallAsync(new RawPublicRequests.GetChatsRequest(fromDate, region), ct)
            .ConfigureAwait(false);
        var request = new PublicRequests.GetChatsRequest(fromDate, region);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetChats",
            raw => (IReadOnlyList<BitflyerChatNormalized>)raw
                .Select(BitflyerChatNormalizer.Normalize)
                .ToArray());
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
            "Bitflyer.GetCorporateLeverage",
            raw => new BitflyerCorporateLeverageNormalized(
                raw.CurrentMax,
                raw.CurrentStartDate,
                raw.NextMax,
                raw.NextStartDate));
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
            "Bitflyer.GetFundingRate",
            raw => new BitflyerFundingRateNormalized(
                raw.CurrentFundingRate,
                raw.NextFundingRateSettleDate));
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
