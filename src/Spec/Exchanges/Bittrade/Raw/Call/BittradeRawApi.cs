using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Spec.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private/Trading をまとめた単一入口）。
/// </summary>
public sealed class BittradeRawApi : IBittradeRawApi
{
    private readonly IBittradePublicApi _publicApi;
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;

    public IBittradeRawMarketDataApi MarketData { get; }
    public IBittradeRawTradingApi Trading { get; }

    public BittradeRawApi(IWireTransport wire)
        : this(
            publicApi: new BittradePublicApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            privateApi: new BittradePrivateApi(wire),
            privateTradingApi: new BittradePrivateTradingApi(wire))
    {
    }

    internal BittradeRawApi(
        IBittradePublicApi publicApi,
        IBittradePrivateApi privateApi,
        IBittradePrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        MarketData = new BittradeRawMarketDataApi(_publicApi);
        Trading = new BittradeRawTradingApi(_privateApi, _privateTradingApi);
    }

    public async Task<Call<GetRawSymbolsRequest, RawSymbolsResponse>> GetSymbolsAsync(
        GetRawSymbolsRequest request,
        CancellationToken cancellationToken = default)
    {
        var inner = await _publicApi.GetSymbolsAsync(new GetSymbolsRequest(), cancellationToken).ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetSymbols", inner);
    }

    public async Task<Call<GetRawTimestampRequest, RawTimestampResponse>> GetTimestampAsync(
        GetRawTimestampRequest request,
        CancellationToken cancellationToken = default)
    {
        var inner = await _publicApi.GetTimestampAsync(new GetTimestampRequest(), cancellationToken).ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetTimestamp", inner);
    }

    public async Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountBalanceAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var inner = await _privateApi
            .GetAccountBalanceAsync(new GetAccountBalanceRequest(request.AccountId), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetAccountBalance", inner);
    }

    private static Call<TReq, TRes> MapCall<TReq, TRes, TOtherReq>(
        TReq request,
        string component,
        Call<TOtherReq, TRes> inner)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: component,
            Tags: null,
            Children: new[] { inner.Id });

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: inner.StartedAt,
            Duration: inner.Duration,
            Request: request,
            Result: inner.Result,
            Meta: meta);
    }
}
