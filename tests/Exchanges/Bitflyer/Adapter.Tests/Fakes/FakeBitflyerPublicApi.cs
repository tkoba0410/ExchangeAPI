using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

internal sealed class FakeBitflyerPublicApi : IBitflyerRawApi
{
    private readonly RawPublicDtos.Ticker _response;
    private readonly RawPublicDtos.Board? _board;
    private readonly FakeBitflyerPrivateApi? _privateApi;
    private readonly FakeBitflyerPrivateTradingApi? _tradingApi;

    public FakeBitflyerPublicApi(
        RawPublicDtos.Ticker response,
        RawPublicDtos.Board? board = null,
        FakeBitflyerPrivateApi? privateApi = null,
        FakeBitflyerPrivateTradingApi? tradingApi = null)
    {
        _response = response;
        _board = board;
        _privateApi = privateApi;
        _tradingApi = tradingApi;
    }

    public Task<Call<RawPublicRequests.GetMarketsRequest, IReadOnlyList<RawPublicDtos.Market>>> GetMarketsCallAsync(
        RawPublicRequests.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPublicDtos.Market>)new[] { new RawPublicDtos.Market("BTC_JPY", "BTC_JPY") }));

    public Task<Call<RawPublicRequests.GetBoardRequest, RawPublicDtos.Board>> GetBoardCallAsync(
        RawPublicRequests.GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_board is null)
        {
            throw new InvalidOperationException("RawPublicDtos.Board response is not configured.");
        }

        return Task.FromResult(MakeOkCall(request, _board));
    }

    public Task<Call<RawPublicRequests.GetTickerRequest, RawPublicDtos.Ticker>> GetTickerCallAsync(
        RawPublicRequests.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<RawPublicRequests.GetExecutionsRequest, IReadOnlyList<RawPublicDtos.ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        RawPublicRequests.GetExecutionsRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RawPublicDtos.ExecutionPublicResponse> executions = new[]
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

    public Task<Call<RawPublicRequests.GetBoardStateRequest, RawPublicDtos.BoardStateResponse>> GetBoardStateCallAsync(
        RawPublicRequests.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.BoardStateResponse("NORMAL", "RUNNING", null)));

    public Task<Call<RawPublicRequests.GetHealthRequest, RawPublicDtos.HealthResponse>> GetHealthCallAsync(
        RawPublicRequests.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.HealthResponse("NORMAL")));

    public Task<Call<RawPublicRequests.GetFundingRateRequest, RawPublicDtos.FundingRateResponse>> GetFundingRateCallAsync(
        RawPublicRequests.GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.FundingRateResponse(0m, DateTimeOffset.UtcNow)));

    public Task<Call<RawPublicRequests.GetCorporateLeverageRequest, RawPublicDtos.CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        RawPublicRequests.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicDtos.CorporateLeverageResponse(
            CurrentMax: 7.7m,
            CurrentStartDate: DateTimeOffset.UtcNow,
            NextMax: 7.65m,
            NextStartDate: DateTimeOffset.UtcNow.AddDays(7))));

    public Task<Call<RawPublicRequests.GetChatsRequest, IReadOnlyList<RawPublicDtos.Chat>>> GetChatsCallAsync(
        RawPublicRequests.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPublicDtos.Chat>)new[] { new RawPublicDtos.Chat("n", "m", DateTimeOffset.UtcNow) }));

    public Task<Call<RawPrivateRequests.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        RawPrivateRequests.GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetPermissionsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetBalancesRequest, IReadOnlyList<RawPrivateDtos.BalanceResponse>>> GetBalanceCallAsync(
        RawPrivateRequests.GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBalanceCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCollateralRequest, RawPrivateDtos.CollateralResponse>> GetCollateralCallAsync(
        RawPrivateRequests.GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCollateralAccountsRequest, IReadOnlyList<RawPrivateDtos.CollateralAccount>>> GetCollateralAccountsCallAsync(
        RawPrivateRequests.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralAccountsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetAddressesRequest, RawPrivateDtos.RawJsonResponse>> GetAddressesCallAsync(
        RawPrivateRequests.GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetAddressesCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCoinInsRequest, RawPrivateDtos.RawJsonResponse>> GetCoinInsCallAsync(
        RawPrivateRequests.GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCoinInsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCoinOutsRequest, RawPrivateDtos.RawJsonResponse>> GetCoinOutsCallAsync(
        RawPrivateRequests.GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCoinOutsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetBankAccountsRequest, RawPrivateDtos.RawJsonResponse>> GetBankAccountsCallAsync(
        RawPrivateRequests.GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBankAccountsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetDepositsRequest, RawPrivateDtos.RawJsonResponse>> GetDepositsCallAsync(
        RawPrivateRequests.GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetDepositsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CreateWithdrawalRequest, RawPrivateDtos.CreateWithdrawalResponse>> WithdrawCallAsync(
        RawPrivateRequests.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetWithdrawalsRequest, RawPrivateDtos.RawJsonResponse>> GetWithdrawalsCallAsync(
        RawPrivateRequests.GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetWithdrawalsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CreateChildOrderRequest, RawPrivateDtos.RawSendChildOrderResponse>> SendChildOrderCallAsync(
        RawPrivateRequests.CreateChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.SendChildOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CreateParentOrderRequest, RawPrivateDtos.RawSendParentOrderResponse>> SendParentOrderCallAsync(
        RawPrivateRequests.CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.SendParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CancelChildOrderRequest, RawPrivateDtos.RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        RawPrivateRequests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.CancelChildOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CancelParentOrderRequest, RawPrivateDtos.RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        RawPrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.CancelParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.CancelAllChildOrdersRequest, RawPrivateDtos.RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        RawPrivateRequests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetChildOrdersRequest, IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        RawPrivateRequests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetChildOrdersCallAsync(request, cancellationToken)
        ?? _privateApi?.GetChildOrdersCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetParentOrdersRequest, IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        RawPrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetParentOrdersCallAsync(request, cancellationToken)
        ?? _privateApi?.GetParentOrdersCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetParentOrderRequest, RawPrivateDtos.RawGetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetParentOrderCallAsync(request, cancellationToken)
        ?? _privateApi?.GetParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetAccountExecutionsRequest, IReadOnlyList<RawPrivateDtos.ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        RawPrivateRequests.GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetExecutionsPrivateCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetBalanceHistoryRequest, RawPrivateDtos.RawJsonResponse>> GetBalanceHistoryCallAsync(
        RawPrivateRequests.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBalanceHistoryCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetPositionsRequest, IReadOnlyList<RawPrivateDtos.PositionResponse>>> GetPositionsCallAsync(
        RawPrivateRequests.GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetPositionsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetCollateralHistoryRequest, RawPrivateDtos.RawJsonResponse>> GetCollateralHistoryCallAsync(
        RawPrivateRequests.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralHistoryCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateRequests.GetTradingCommissionRequest, RawPrivateDtos.RawJsonResponse>> GetTradingCommissionCallAsync(
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
