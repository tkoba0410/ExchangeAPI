using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using BittradeRequests = ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;

internal sealed class BittradeRawMarketDataApi : IBittradeRawMarketDataApi
{
    private readonly IBittradePublicApi _publicApi;

    public BittradeRawMarketDataApi(IBittradePublicApi publicApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
    }

    public async Task<Call<BittradeRequests.GetTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        BittradeRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetDetailMergedCallAsync(new GetMergedTickerRequest(request.Symbol), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetTicker", publicCall);
    }

    public async Task<Call<BittradeRequests.GetOrderBookRequest, RawDepthResponse>> GetDepthCallAsync(
        BittradeRequests.GetOrderBookRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetDepthCallAsync(new GetDepthRequest(request.Symbol, request.Type), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetOrderBook", publicCall);
    }

    public async Task<Call<BittradeRequests.GetMarketTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        BittradeRequests.GetMarketTradesRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetTradeCallAsync(new GetTradesRequest(request.Symbol), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetTrades", publicCall);
    }

    private static Call<TReq, TRes> MapCall<TReq, TRes, TOtherReq>(
        TReq request,
        string component,
        Call<TOtherReq, TRes> publicCall)
    {
        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: publicCall.StartedAt,
            Duration: publicCall.Duration,
            Request: request,
            Result: publicCall.Result,
            Meta: publicCall.Meta);
    }

}
