using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

internal sealed class BittradeRawExchangeInfoApi : IBittradeRawExchangeInfoApi
{
    private readonly IBittradePublicApi _publicApi;

    public BittradeRawExchangeInfoApi(IBittradePublicApi publicApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
    }

    public async Task<Call<GetRawSymbolsRequest, RawSymbolsResponse>> GetSymbolsAsync(
        GetRawSymbolsRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetSymbolsAsync(new GetSymbolsRequest(), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetSymbols", publicCall);
    }

    public async Task<Call<GetRawTimestampRequest, RawTimestampResponse>> GetTimestampAsync(
        GetRawTimestampRequest request,
        CancellationToken cancellationToken = default)
    {
        var publicCall = await _publicApi
            .GetTimestampAsync(new GetTimestampRequest(), cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetTimestamp", publicCall);
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
            Children: new[] { publicCall.Id })
        {
            RawJson = publicCall.Meta.RawJson
        };

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: publicCall.StartedAt,
            Duration: publicCall.Duration,
            Request: request,
            Result: publicCall.Result,
            Meta: meta);
    }
}
