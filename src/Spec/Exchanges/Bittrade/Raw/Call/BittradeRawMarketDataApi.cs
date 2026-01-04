using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

internal sealed class BittradeRawMarketDataApi : IBittradeRawMarketDataApi
{
    private readonly IBittradePublicApi _publicApi;

    public BittradeRawMarketDataApi(IBittradePublicApi publicApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
    }

    public async Task<Call<GetTickerRequest, RawMergedResponse>> GetTickerAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetMergedTickerAsync(new GetMergedTickerRequest(request.Symbol), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetTicker", publicCall);
    }

    public async Task<Call<GetOrderBookRequest, RawDepthResponse>> GetOrderBookAsync(
        GetOrderBookRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetDepthAsync(new GetDepthRequest(request.Symbol, request.Type), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetOrderBook", publicCall);
    }

    public async Task<Call<GetMarketTradesRequest, RawTradeResponse>> GetTradesAsync(
        GetMarketTradesRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetTradesAsync(new GetTradesRequest(request.Symbol), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetTrades", publicCall);
    }

    private static Call<TReq, TRes> MapCall<TReq, TRes, TOtherReq>(
        TReq request,
        string component,
        Call<TOtherReq, TRes> publicCall)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: component,
            Tags: null,
            Children: new[] { publicCall.Id });

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: publicCall.StartedAt,
            Duration: publicCall.Duration,
            Request: request,
            Result: publicCall.Result,
            Meta: meta);
    }

}
