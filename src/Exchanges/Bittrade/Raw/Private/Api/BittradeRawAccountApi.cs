using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

internal sealed class BittradeRawAccountApi : IBittradeRawAccountApi
{
    private readonly IBittradePrivateApi _privateApi;

    public BittradeRawAccountApi(IBittradePrivateApi privateApi)
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
    }

    public async Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        var privateCall = await _privateApi
            .GetAccountsBalanceByAccountIdCallAsync(request, cancellationToken)
            .ConfigureAwait(false);
        return MapCall(request, "Bittrade.GetAccountBalance", privateCall);
    }

    private static Call<TReq, TRes> MapCall<TReq, TRes, TOtherReq>(
        TReq request,
        string component,
        Call<TOtherReq, TRes> privateCall)
    {
        return new Call<TReq, TRes>(
            Id: CallId.New(),
            StartedAt: privateCall.StartedAt,
            Duration: privateCall.Duration,
            Request: request,
            Result: privateCall.Result,
            Meta: privateCall.Meta);
    }
}
