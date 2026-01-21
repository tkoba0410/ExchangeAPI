using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

internal sealed class FakeBitflyerPublicApi : IBitflyerRawApi
{
    private readonly RawPublicModels.Ticker _response;
    private readonly RawPublicModels.Board? _board;
    private readonly FakeBitflyerPrivateApi? _privateApi;
    private readonly FakeBitflyerPrivateTradingApi? _tradingApi;

    public FakeBitflyerPublicApi(
        RawPublicModels.Ticker response,
        RawPublicModels.Board? board = null,
        FakeBitflyerPrivateApi? privateApi = null,
        FakeBitflyerPrivateTradingApi? tradingApi = null)
    {
        _response = response;
        _board = board;
        _privateApi = privateApi;
        _tradingApi = tradingApi;
    }

    public Task<Call<RawPublicModels.GetMarketsRequest, IReadOnlyList<RawPublicModels.Market>>> GetMarketsCallAsync(
        RawPublicModels.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPublicModels.Market>)new[] { new RawPublicModels.Market("BTC_JPY", "BTC_JPY") }));

    public Task<Call<RawPublicModels.GetBoardRequest, RawPublicModels.Board>> GetBoardCallAsync(
        RawPublicModels.GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_board is null)
        {
            throw new InvalidOperationException("RawPublicModels.Board response is not configured.");
        }

        return Task.FromResult(MakeOkCall(request, _board));
    }

    public Task<Call<RawPublicModels.GetTickerRequest, RawPublicModels.Ticker>> GetTickerCallAsync(
        RawPublicModels.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<RawPublicModels.GetExecutionsRequest, IReadOnlyList<RawPublicModels.ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
        RawPublicModels.GetExecutionsRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RawPublicModels.ExecutionPublicResponse> executions = new[]
        {
            new RawPublicModels.ExecutionPublicResponse
            {
                Id = 1,
                ProductCode = request.ProductCode,
                Side = "BUY",
                Price = 100m,
                Size = 0.01m,
                ExecDate = DateTimeOffset.UtcNow,
            }
        };

        return Task.FromResult(MakeOkCall(request, executions));
    }

    public Task<Call<RawPublicModels.GetBoardStateRequest, RawPublicModels.BoardStateResponse>> GetBoardStateCallAsync(
        RawPublicModels.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.BoardStateResponse("NORMAL", "RUNNING", null)));

    public Task<Call<RawPublicModels.GetHealthRequest, RawPublicModels.HealthResponse>> GetHealthCallAsync(
        RawPublicModels.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.HealthResponse("NORMAL")));

    public Task<Call<RawPublicModels.GetFundingRateRequest, RawPublicModels.FundingRateResponse>> GetFundingRateCallAsync(
        RawPublicModels.GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.FundingRateResponse(0m, DateTimeOffset.UtcNow)));

    public Task<Call<RawPublicModels.GetCorporateLeverageRequest, RawPublicModels.CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        RawPublicModels.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.CorporateLeverageResponse(
            CurrentMax: 7.7m,
            CurrentStartDate: DateTimeOffset.UtcNow,
            NextMax: 7.65m,
            NextStartDate: DateTimeOffset.UtcNow.AddDays(7))));

    public Task<Call<RawPublicModels.GetChatsRequest, IReadOnlyList<RawPublicModels.Chat>>> GetChatsCallAsync(
        RawPublicModels.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPublicModels.Chat>)new[] { new RawPublicModels.Chat("n", "m", DateTimeOffset.UtcNow) }));

    public Task<Call<RawPrivateModels.GetPermissionsRequest, IReadOnlyList<string>>> GetPermissionsCallAsync(
        RawPrivateModels.GetPermissionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetPermissionsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetBalancesRequest, IReadOnlyList<RawPrivateModels.BalanceResponse>>> GetBalanceCallAsync(
        RawPrivateModels.GetBalancesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBalanceCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetCollateralRequest, RawPrivateModels.CollateralResponse>> GetCollateralCallAsync(
        RawPrivateModels.GetCollateralRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetCollateralAccountsRequest, IReadOnlyList<RawPrivateModels.CollateralAccount>>> GetCollateralAccountsCallAsync(
        RawPrivateModels.GetCollateralAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralAccountsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetAddressesRequest, RawPrivateModels.RawJsonResponse>> GetAddressesCallAsync(
        RawPrivateModels.GetAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetAddressesCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetCoinInsRequest, RawPrivateModels.RawJsonResponse>> GetCoinInsCallAsync(
        RawPrivateModels.GetCoinInsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCoinInsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetCoinOutsRequest, RawPrivateModels.RawJsonResponse>> GetCoinOutsCallAsync(
        RawPrivateModels.GetCoinOutsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCoinOutsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetBankAccountsRequest, RawPrivateModels.RawJsonResponse>> GetBankAccountsCallAsync(
        RawPrivateModels.GetBankAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBankAccountsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetDepositsRequest, RawPrivateModels.RawJsonResponse>> GetDepositsCallAsync(
        RawPrivateModels.GetDepositsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetDepositsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.CreateWithdrawalRequest, RawPrivateModels.CreateWithdrawalResponse>> WithdrawCallAsync(
        RawPrivateModels.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetWithdrawalsRequest, RawPrivateModels.RawJsonResponse>> GetWithdrawalsCallAsync(
        RawPrivateModels.GetWithdrawalsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetWithdrawalsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<string, RawPrivateModels.RawSendChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.SendChildOrderCallAsync(bodyJson, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<string, RawPrivateModels.RawSendParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.SendParentOrderCallAsync(bodyJson, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.CancelChildOrderRequest, RawPrivateModels.RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        RawPrivateModels.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.CancelChildOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.CancelParentOrderRequest, RawPrivateModels.RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        RawPrivateModels.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.CancelParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.CancelAllChildOrdersRequest, RawPrivateModels.RawCancelAllChildOrdersResponse>> CancelAllChildOrdersCallAsync(
        RawPrivateModels.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetChildOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        RawPrivateModels.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetChildOrdersCallAsync(request, cancellationToken)
        ?? _privateApi?.GetChildOrdersCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetParentOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        RawPrivateModels.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetParentOrdersCallAsync(request, cancellationToken)
        ?? _privateApi?.GetParentOrdersCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.RawGetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateModels.GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _tradingApi?.GetParentOrderCallAsync(request, cancellationToken)
        ?? _privateApi?.GetParentOrderCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetAccountExecutionsRequest, IReadOnlyList<RawPrivateModels.ExecutionPrivateResponse>>> GetExecutionsPrivateCallAsync(
        RawPrivateModels.GetAccountExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetExecutionsPrivateCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetBalanceHistoryRequest, RawPrivateModels.RawJsonResponse>> GetBalanceHistoryCallAsync(
        RawPrivateModels.GetBalanceHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetBalanceHistoryCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetPositionsRequest, IReadOnlyList<RawPrivateModels.PositionResponse>>> GetPositionsCallAsync(
        RawPrivateModels.GetPositionsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetPositionsCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetCollateralHistoryRequest, RawPrivateModels.RawJsonResponse>> GetCollateralHistoryCallAsync(
        RawPrivateModels.GetCollateralHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi?.GetCollateralHistoryCallAsync(request, cancellationToken)
        ?? throw new NotSupportedException();

    public Task<Call<RawPrivateModels.GetTradingCommissionRequest, RawPrivateModels.RawJsonResponse>> GetTradingCommissionCallAsync(
        RawPrivateModels.GetTradingCommissionRequest request,
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
