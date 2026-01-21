using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.Account;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.Trading;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using NormalizedRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using RawPublicModels = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Api;

public sealed class BitflyerNormalizedApi : IBitflyerNormalizedApi
{
    private readonly BitflyerNormalizedMarketDataFacade _marketData;
    private readonly BitflyerNormalizedExchangeInfoFacade _exchangeInfo;
    private readonly BitflyerNormalizedTradingApi _trading;
    private readonly BitflyerNormalizedAccountApi _account;

    private BitflyerNormalizedApi(
        BitflyerNormalizedMarketDataFacade marketData,
        BitflyerNormalizedExchangeInfoFacade exchangeInfo,
        BitflyerNormalizedTradingApi trading,
        BitflyerNormalizedAccountApi account)
    {
        _marketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        _exchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _account = account ?? throw new ArgumentNullException(nameof(account));
    }

    public static BitflyerNormalizedApi FromRestClient(IRestClient restClient, IBitflyerMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));
        var wire = new WireTransport(restClient);
        var raw = new BitflyerRawApi(wire);

        return FromRaw(raw, markets);
    }

    internal static BitflyerNormalizedApi FromRaw(IBitflyerRawApi raw, IBitflyerMarketResolver markets)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        return new BitflyerNormalizedApi(
            marketData: new BitflyerNormalizedMarketDataFacade(raw),
            exchangeInfo: new BitflyerNormalizedExchangeInfoFacade(raw),
            trading: new BitflyerNormalizedTradingApi(raw, markets),
            account: new BitflyerNormalizedAccountApi(raw, markets));
    }

    public Task<Call<NormalizedRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        string? region = null,
        CancellationToken cancellationToken = default) =>
        _exchangeInfo.GetMarketsCallAsync(region, cancellationToken);

    public Task<Call<NormalizedRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetTickerCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetBoardCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetExecutionsPublicCallAsync(productCode, count, before, after, cancellationToken);

    public Task<Call<NormalizedRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetHealthCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken cancellationToken = default) =>
        _marketData.GetBoardStateCallAsync(productCode, cancellationToken);

    public Task<Call<NormalizedRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default) =>
        _marketData.GetChatsCallAsync(fromDate, region, cancellationToken);

    public Task<Call<NormalizedRequests.PlaceOrderRequest, BitflyerOrderResult>> PlaceOrderCallAsync(
        BitflyerOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.PlaceOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.CancelOrderRequest, BitflyerCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.CancelOrderCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<NormalizedRequests.GetOpenOrdersRequest, IReadOnlyList<BitflyerOpenOrder>>> GetChildOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _trading.GetChildOrdersCallAsync(symbol, cancellationToken);

    public Task<Call<NormalizedRequests.GetOrderRequest, BitflyerOrderStatus>> GetChildOrdersCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        _trading.GetChildOrdersCallAsync(symbol, orderKey, cancellationToken);

    public Task<Call<NormalizedRequests.SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        NormalizedRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.SendParentOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        NormalizedRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.CancelParentOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        NormalizedRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.GetParentOrdersCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        NormalizedRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.GetParentOrderCallAsync(request, cancellationToken);

    public Task<Call<NormalizedRequests.GetBalancesRequest, IReadOnlyList<BitflyerBalanceEntryNormalized>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _account.GetBalanceCallAsync(cancellationToken);

    public Task<Call<NormalizedRequests.GetAccountExecutionsRequest, IReadOnlyList<BitflyerExecutionAccountNormalized>>> GetExecutionsPrivateCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetExecutionsPrivateCallAsync(symbol, cancellationToken);

    public Task<Call<NormalizedRequests.GetTradingCommissionRequest, BitflyerTradingCommissionNormalized>> GetTradingCommissionCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        _account.GetTradingCommissionCallAsync(symbol, cancellationToken);
}

internal sealed class BitflyerNormalizedMarketDataFacade
{
    private readonly IBitflyerRawApi _raw;

    internal BitflyerNormalizedMarketDataFacade(IBitflyerRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<NormalizedRequests.GetTickerRequest, BitflyerTickerNormalized>> GetTickerCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetTickerCallAsync(new RawPublicModels.GetTickerRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetTickerRequest(productCode);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetTicker",
            raw => BitflyerTickerNormalizer.Normalize(raw, rawCall.Meta.RawJson));
    }

    public async Task<Call<NormalizedRequests.GetOrderBookRequest, BitflyerOrderBookNormalized>> GetBoardCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardCallAsync(new RawPublicModels.GetBoardRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetOrderBookRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetBoard", BitflyerOrderBookNormalizer.Normalize);
    }

    public async Task<Call<NormalizedRequests.GetExecutionsRequest, IReadOnlyList<BitflyerExecutionNormalized>>> GetExecutionsPublicCallAsync(
        string productCode,
        int? count = null,
        long? before = null,
        long? after = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetExecutionsPublicCallAsync(new RawPublicModels.GetExecutionsRequest(productCode, count, before, after), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetExecutionsRequest(productCode, count, before, after);

        return CreateCall(
            rawCall,
            request,
            "Bitflyer.GetExecutions",
            raw => (IReadOnlyList<BitflyerExecutionNormalized>)BitflyerExecutionNormalizer.NormalizeList(
                raw,
                rawCall.Meta.RawJson));
    }

    public async Task<Call<NormalizedRequests.GetHealthRequest, BitflyerHealthNormalized>> GetHealthCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetHealthCallAsync(new RawPublicModels.GetHealthRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetHealthRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetHealth", BitflyerHealthNormalizer.Normalize);
    }

    public async Task<Call<NormalizedRequests.GetBoardStateRequest, BitflyerBoardStateNormalized>> GetBoardStateCallAsync(
        string productCode,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetBoardStateCallAsync(new RawPublicModels.GetBoardStateRequest(productCode), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetBoardStateRequest(productCode);

        return CreateCall(rawCall, request, "Bitflyer.GetBoardState", BitflyerBoardStateNormalizer.Normalize);
    }

    public async Task<Call<NormalizedRequests.GetChatsRequest, IReadOnlyList<BitflyerChatNormalized>>> GetChatsCallAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetChatsCallAsync(new RawPublicModels.GetChatsRequest(fromDate, region), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetChatsRequest(fromDate, region);

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

internal sealed class BitflyerNormalizedExchangeInfoFacade
{
    private readonly IBitflyerRawApi _raw;

    internal BitflyerNormalizedExchangeInfoFacade(IBitflyerRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<Call<NormalizedRequests.GetMarketsRequest, IReadOnlyList<BitflyerMarketNormalized>>> GetMarketsCallAsync(
        string? region = null,
        CancellationToken ct = default)
    {
        var rawCall = await _raw
            .GetMarketsCallAsync(new RawPublicModels.GetMarketsRequest(region), ct)
            .ConfigureAwait(false);
        var request = new NormalizedRequests.GetMarketsRequest(region);

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
