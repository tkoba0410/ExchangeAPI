using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

internal sealed class BittradeRawAccountApi : IBittradeRawAccountApi
{
    private readonly IBittradePrivateApi _privateApi;

    public BittradeRawAccountApi(IBittradePrivateApi privateApi)
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
    }

    public async Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountBalanceAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var privateCall = await _privateApi
            .GetAccountBalanceAsync(request, cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetAccountBalance", privateCall);
    }

    private static Call<TReq, TRes> MapCall<TReq, TRes, TOtherReq>(
        TReq request,
        string component,
        Call<TOtherReq, TRes> privateCall)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: component,
            Tags: null,
            Children: new[] { privateCall.Id })
        {
            RawJson = privateCall.Meta.RawJson
        };

        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: privateCall.StartedAt,
            Duration: privateCall.Duration,
            Request: request,
            Result: privateCall.Result,
            Meta: meta);
    }
}
