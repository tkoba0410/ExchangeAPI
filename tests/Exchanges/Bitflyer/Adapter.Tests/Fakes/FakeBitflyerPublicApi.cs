using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

internal sealed class FakeBitflyerPublicApi : IBitflyerRawApi
{
    private readonly RawPublicDtos.GetTickerResponse _response;
    private readonly RawPublicDtos.GetBoardResponse? _board;
    private readonly FakeBitflyerPrivateApi? _privateApi;
    private readonly FakeBitflyerPrivateTradingApi? _tradingApi;

    public FakeBitflyerPublicApi(
        RawPublicDtos.GetTickerResponse response,
        RawPublicDtos.GetBoardResponse? board = null,
        FakeBitflyerPrivateApi? privateApi = null,
        FakeBitflyerPrivateTradingApi? tradingApi = null)
    {
        _response = response;
        _board = board;
        _privateApi = privateApi;
        _tradingApi = tradingApi;
    }

    public Task<Call<RawPublicRequests.GetMarketsRequest, RawPublicDtos.GetMarketsResponse>> GetMarketsCallAsync(
        RawPublicRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPublicDtos.GetMarketsResponse
        {
            new RawPublicDtos.Market("BTC_JPY", "BTC_JPY")
        };
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPublicRequests.GetBoardRequest, RawPublicDtos.GetBoardResponse>> GetBoardCallAsync(
        RawPublicRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_board is null)
        {
            throw new InvalidOperationException("RawPublicDtos.GetBoardResponse response is not configured.");
        }

        return Task.FromResult(MakeOkCall(request, _board));
    }

    public Task<Call<RawPublicRequests.GetTickerRequest, RawPublicDtos.GetTickerResponse>> GetTickerCallAsync(
        RawPublicRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<RawPublicRequests.GetExecutionsPublicRequest, RawPublicDtos.GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        RawPublicRequests.GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        var executions = new RawPublicDtos.GetExecutionsPublicResponse
        {
            new RawPublicDtos.ExecutionPublicResponse
            {
                Id = 1,
                ProductCode = request.ProductCode.Value,
                Side = "BUY",
                Price = 100m,
                Size = 0.01m,
                ExecDate = DateTimeOffset.UtcNow,
            }
        };

        return Task.FromResult(MakeOkCall(request, executions));
    }

    public Task<Call<RawPublicRequests.GetBoardStateRequest, RawPublicDtos.GetBoardStateResponse>> GetBoardStateCallAsync(
        RawPublicRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.GetBoardStateResponse("NORMAL", "RUNNING", null)));

    public Task<Call<RawPublicRequests.GetHealthRequest, RawPublicDtos.GetHealthResponse>> GetHealthCallAsync(
        RawPublicRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.GetHealthResponse("NORMAL")));

    public Task<Call<RawPublicRequests.GetFundingRateRequest, RawPublicDtos.GetFundingRateResponse>> GetFundingRateCallAsync(
        RawPublicRequests.GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.GetFundingRateResponse(0m, DateTimeOffset.UtcNow)));

    public Task<Call<RawPublicRequests.GetCorporateLeverageRequest, RawPublicDtos.GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        RawPublicRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.GetCorporateLeverageResponse(
            CurrentMax: 7.7m,
            CurrentStartDate: DateTimeOffset.UtcNow,
            NextMax: 7.65m,
            NextStartDate: DateTimeOffset.UtcNow.AddDays(7))));

    public Task<Call<RawPublicRequests.GetChatsRequest, RawPublicDtos.GetChatsResponse>> GetChatsCallAsync(
        RawPublicRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPublicDtos.GetChatsResponse
        {
            new RawPublicDtos.Chat("n", "m", DateTimeOffset.UtcNow)
        };
        return Task.FromResult(MakeOkCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetPermissionsRequest, RawPrivateDtos.GetPermissionsResponse>> GetPermissionsCallAsync(
        RawPrivateRequests.GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetPermissionsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetBalanceRequest, RawPrivateDtos.GetBalanceResponse>> GetBalanceCallAsync(
        RawPrivateRequests.GetBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBalanceCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCollateralRequest, RawPrivateDtos.GetCollateralResponse>> GetCollateralCallAsync(
        RawPrivateRequests.GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCollateralAccountsRequest, RawPrivateDtos.GetCollateralAccountsResponse>> GetCollateralAccountsCallAsync(
        RawPrivateRequests.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralAccountsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetAddressesRequest, RawPrivateDtos.GetAddressesResponse>> GetAddressesCallAsync(
        RawPrivateRequests.GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetAddressesCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCoinInsRequest, RawPrivateDtos.GetCoinInsResponse>> GetCoinInsCallAsync(
        RawPrivateRequests.GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCoinInsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCoinOutsRequest, RawPrivateDtos.GetCoinOutsResponse>> GetCoinOutsCallAsync(
        RawPrivateRequests.GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCoinOutsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetBankAccountsRequest, RawPrivateDtos.GetBankAccountsResponse>> GetBankAccountsCallAsync(
        RawPrivateRequests.GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBankAccountsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetDepositsRequest, RawPrivateDtos.GetDepositsResponse>> GetDepositsCallAsync(
        RawPrivateRequests.GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetDepositsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetExecutionsPrivateRequest, RawPrivateDtos.GetExecutionsPrivateResponse>> GetExecutionsPrivateCallAsync(
        RawPrivateRequests.GetExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetExecutionsPrivateCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.WithdrawRequest, RawPrivateDtos.WithdrawResponse>> WithdrawCallAsync(
        RawPrivateRequests.WithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetWithdrawalsRequest, RawPrivateDtos.GetWithdrawalsResponse>> GetWithdrawalsCallAsync(
        RawPrivateRequests.GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetWithdrawalsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.SendChildOrderRequest, RawPrivateDtos.SendChildOrderResponse>> SendChildOrderCallAsync(
        RawPrivateRequests.SendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.SendChildOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.SendParentOrderRequest, RawPrivateDtos.SendParentOrderResponse>> SendParentOrderCallAsync(
        RawPrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.SendParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CancelChildOrderRequest, RawPrivateDtos.CancelChildOrderResponse>> CancelChildOrderCallAsync(
        RawPrivateRequests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.CancelChildOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CancelParentOrderRequest, RawPrivateDtos.CancelParentOrderResponse>> CancelParentOrderCallAsync(
        RawPrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.CancelParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CancelAllChildOrdersRequest, RawPrivateDtos.CancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        RawPrivateRequests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetChildOrdersRequest, RawPrivateDtos.GetChildOrdersResponse>> GetChildOrdersCallAsync(
        RawPrivateRequests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetChildOrdersCallAsync(request, cancellationToken)
        ?? _privateApi?.GetChildOrdersCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetParentOrdersRequest, RawPrivateDtos.GetParentOrdersResponse>> GetParentOrdersCallAsync(
        RawPrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetParentOrdersCallAsync(request, cancellationToken)
        ?? _privateApi?.GetParentOrdersCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetParentOrderRequest, RawPrivateDtos.GetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetParentOrderCallAsync(request, cancellationToken)
        ?? _privateApi?.GetParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetBalanceHistoryRequest, RawPrivateDtos.GetBalanceHistoryResponse>> GetBalanceHistoryCallAsync(
        RawPrivateRequests.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBalanceHistoryCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetPositionsRequest, RawPrivateDtos.GetPositionsResponse>> GetPositionsCallAsync(
        RawPrivateRequests.GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetPositionsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCollateralHistoryRequest, RawPrivateDtos.GetCollateralHistoryResponse>> GetCollateralHistoryCallAsync(
        RawPrivateRequests.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralHistoryCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetTradingCommissionRequest, RawPrivateDtos.GetTradingCommissionResponse>> GetTradingCommissionCallAsync(
        RawPrivateRequests.GetTradingCommissionRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetTradingCommissionCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
    {
        var meta = CallMeta.CreateInternal("Raw", "FakeBitflyerPublicApi");
        return new Call<TReq, TResponse>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Ok(response),
            Meta: meta);
    }
}
