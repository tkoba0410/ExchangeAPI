using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Spec.Wire;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Call;

public sealed class BitflyerNormalizedApi
{
    public BitflyerNormalizedMarketDataFacade MarketData { get; }
    public BitflyerNormalizedExchangeInfoFacade ExchangeInfo { get; }

    private BitflyerNormalizedApi(
        BitflyerNormalizedMarketDataFacade marketData,
        BitflyerNormalizedExchangeInfoFacade exchangeInfo)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
    }

    public static BitflyerNormalizedApi FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var wire = new WireTransport(restClient);
        var raw = new BitflyerRawApi(wire);

        return new BitflyerNormalizedApi(
            marketData: new BitflyerNormalizedMarketDataFacade(raw.MarketData),
            exchangeInfo: new BitflyerNormalizedExchangeInfoFacade(raw.MarketData));
    }
}

public sealed class BitflyerNormalizedMarketDataFacade
{
    private readonly IBitflyerRawMarketDataApi _raw;

    internal BitflyerNormalizedMarketDataFacade(IBitflyerRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<BitflyerTickerNormalized> GetTickerAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var call = await GetTickerCallAsync(productCode, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetTicker");
    }

    public async Task<Call<GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTickerAsync(new RawRequests.GetTickerRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new GetTickerRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetTicker", BitflyerTickerNormalizer.Normalize);
    }

    public async Task<BitflyerOrderBookNormalized> GetOrderBookAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var call = await GetOrderBookCallAsync(productCode, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetBoard");
    }

    public async Task<Call<GetOrderBookRequest, BitflyerOrderBookNormalized>> GetOrderBookCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardAsync(new RawRequests.GetBoardRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new GetOrderBookRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetBoard", BitflyerOrderBookNormalizer.Normalize);
    }

    public async Task<IReadOnlyList<BitflyerExecutionNormalized>> GetExecutionsAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var call = await GetExecutionsCallAsync(productCode, count, before, after, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetExecutions");
    }

    public async Task<Call<GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetExecutionsAsync(new RawRequests.GetExecutionsRequest(productCode, count, before, after), ct)
            .ConfigureAwait(false);
        var request = new GetExecutionsRequest(productCode, count, before, after);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutions",
            raw => (IReadOnlyList<BitflyerExecutionNormalized>)raw
                .Select(BitflyerExecutionNormalizer.Normalize)
                .ToArray());
    }

    public async Task<BitflyerHealthNormalized> GetHealthAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var call = await GetHealthCallAsync(productCode, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetHealth");
    }

    public async Task<Call<GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetHealthAsync(new RawRequests.GetHealthRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new GetHealthRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetHealth", BitflyerHealthNormalizer.Normalize);
    }

    public async Task<BitflyerBoardStateNormalized> GetBoardStateAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var call = await GetBoardStateCallAsync(productCode, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetBoardState");
    }

    public async Task<Call<GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardStateAsync(new RawRequests.GetBoardStateRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new GetBoardStateRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetBoardState", BitflyerBoardStateNormalizer.Normalize);
    }

    public async Task<IReadOnlyList<BitflyerChatNormalized>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var call = await GetChatsCallAsync(fromDate, region, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetChats");
    }

    public async Task<Call<GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetChatsAsync(new RawRequests.GetChatsRequest(fromDate, region), ct)
            .ConfigureAwait(false);
        var request = new GetChatsRequest(fromDate, region);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetChats",
            raw => (IReadOnlyList<BitflyerChatNormalized>)raw
                .Select(BitflyerChatNormalizer.Normalize)
                .ToArray());
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
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: err.Error.HttpStatus is int status ? (HttpStatusCode?)status : null,
                innerException: err.Error.Exception),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}

public sealed class BitflyerNormalizedExchangeInfoFacade
{
    private readonly IBitflyerRawMarketDataApi _raw;

    internal BitflyerNormalizedExchangeInfoFacade(IBitflyerRawMarketDataApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<IReadOnlyList<BitflyerMarketNormalized>> GetMarketsAsync(
        string? region = null,
        CancellationToken ct = default)
    {
        var call = await GetMarketsCallAsync(region, ct).ConfigureAwait(false);
        return Unwrap(call, "Bitflyer.GetMarkets");
    }

    public async Task<Call<GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetMarketsAsync(new RawRequests.GetMarketsRequest(region), ct)
            .ConfigureAwait(false);
        var request = new GetMarketsRequest(region);

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
                exchange: ExchangeCode.Bitflyer,
                operation: operation,
                statusCode: err.Error.HttpStatus is int status ? (HttpStatusCode?)status : null,
                innerException: err.Error.Exception),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}
