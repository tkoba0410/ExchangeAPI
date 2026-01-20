using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

internal sealed class FakeBitflyerPublicApi : IBitflyerRawMarketDataApi
{
    private readonly Ticker _response;
    private readonly Board? _board;

    public FakeBitflyerPublicApi(Ticker response, Board? board = null)
    {
        _response = response;
        _board = board;
    }

    public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<GetTickerRequest, Ticker>> TickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        GetTickerCallAsync(request, cancellationToken);

    public Task<Call<GetBoardRequest, Board>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        if (_board is null)
        {
            throw new InvalidOperationException("Board response is not configured.");
        }

        return Task.FromResult(MakeOkCall(request, _board));
    }

    public Task<Call<GetBoardRequest, Board>> BoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        GetBoardCallAsync(request, cancellationToken);

    public Task<Call<GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> GetExecutionsPublicCallAsync(
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

    public Task<Call<GetExecutionsRequest, IReadOnlyList<ExecutionPublicResponse>>> ExecutionsCallAsync(
        GetExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        GetExecutionsPublicCallAsync(request, cancellationToken);

    public Task<Call<GetMarketsRequest, IReadOnlyList<Market>>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<Market>)new[] { new Market("BTC_JPY", "BTC_JPY") }));

    public Task<Call<GetMarketsRequest, IReadOnlyList<Market>>> MarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        GetMarketsCallAsync(request, cancellationToken);

    public Task<Call<GetChatsRequest, IReadOnlyList<Chat>>> GetChatsCallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<Chat>)new[] { new Chat("n", "m", DateTimeOffset.UtcNow) }));

    public Task<Call<GetHealthRequest, HealthResponse>> GetHealthCallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new HealthResponse("NORMAL")));

    public Task<Call<GetBoardStateRequest, BoardStateResponse>> GetBoardStateCallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new BoardStateResponse("NORMAL", "RUNNING", null)));

    public Task<Call<GetCorporateLeverageRequest, CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new CorporateLeverageResponse(
            CurrentMax: 7.7m,
            CurrentStartDate: DateTimeOffset.UtcNow,
            NextMax: 7.65m,
            NextStartDate: DateTimeOffset.UtcNow.AddDays(7))));

    public Task<Call<GetFundingRateRequest, FundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new FundingRateResponse(0m, DateTimeOffset.UtcNow)));

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
