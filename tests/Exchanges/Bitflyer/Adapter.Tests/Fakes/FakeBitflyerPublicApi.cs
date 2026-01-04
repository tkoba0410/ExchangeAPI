using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;

internal sealed class FakeBitflyerPublicApi : IBitflyerRawMarketDataApi
{
    private readonly Ticker _response;
    private readonly Board? _board;

    public FakeBitflyerPublicApi(Ticker response, Board? board = null)
    {
        _response = response;
        _board = board;
    }

    public Task<Call<GetTickerRequest, Ticker>> GetTickerAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<GetBoardRequest, Board>> GetBoardAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_board is null)
        {
            throw new InvalidOperationException("Board response is not configured.");
        }

        return Task.FromResult(MakeOkCall(request, _board));
    }

    public Task<Call<GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ExecutionPublicResponse> executions = new[]
        {
            new ExecutionPublicResponse
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

    public Task<Call<GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<Market>)new[] { new Market("BTC_JPY", "BTC_JPY") }));

    public Task<Call<GetChatsRequest, IReadOnlyList<Chat>>> GetChatsAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<Chat>)new[] { new Chat("n", "m", DateTimeOffset.UtcNow) }));

    public Task<Call<GetHealthRequest, HealthResponse>> GetHealthAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new HealthResponse("NORMAL")));

    public Task<Call<GetBoardStateRequest, BoardStateResponse>> GetBoardStateAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new BoardStateResponse("NORMAL", "RUNNING", null)));

    public Task<Call<GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new CorporateLeverageResponse(
            CurrentMax: 7.7m,
            CurrentStartDate: DateTimeOffset.UtcNow,
            NextMax: 7.65m,
            NextStartDate: DateTimeOffset.UtcNow.AddDays(7))));

    public Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new FundingRateResponse(0m, DateTimeOffset.UtcNow)));

    private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: "FakeBitflyerPublicApi",
            Tags: null,
            Children: null);
        return new Call<TReq, TResponse>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Ok(response),
            Meta: meta);
    }
}
