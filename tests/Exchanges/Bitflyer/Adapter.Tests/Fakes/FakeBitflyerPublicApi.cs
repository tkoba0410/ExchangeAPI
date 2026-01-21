using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

internal sealed class FakeBitflyerPublicApi : IBitflyerRawMarketDataApi
{
    private readonly RawPublicModels.Ticker _response;
    private readonly RawPublicModels.Board? _board;

    public FakeBitflyerPublicApi(RawPublicModels.Ticker response, RawPublicModels.Board? board = null)
    {
        _response = response;
        _board = board;
    }

    public Task<Call<RawPublicModels.GetTickerRequest, RawPublicModels.Ticker>> GetTickerCallAsync(
        RawPublicModels.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, _response));

    public Task<Call<RawPublicModels.GetTickerRequest, RawPublicModels.Ticker>> TickerCallAsync(
        RawPublicModels.GetTickerRequest request,
        CancellationToken cancellationToken = default) =>
        GetTickerCallAsync(request, cancellationToken);

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

    public Task<Call<RawPublicModels.GetBoardRequest, RawPublicModels.Board>> BoardCallAsync(
        RawPublicModels.GetBoardRequest request,
        CancellationToken cancellationToken = default) =>
        GetBoardCallAsync(request, cancellationToken);

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

    public Task<Call<RawPublicModels.GetExecutionsRequest, IReadOnlyList<RawPublicModels.ExecutionPublicResponse>>> ExecutionsCallAsync(
        RawPublicModels.GetExecutionsRequest request,
        CancellationToken cancellationToken = default) =>
        GetExecutionsPublicCallAsync(request, cancellationToken);

    public Task<Call<RawPublicModels.GetMarketsRequest, IReadOnlyList<RawPublicModels.Market>>> GetMarketsCallAsync(
        RawPublicModels.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPublicModels.Market>)new[] { new RawPublicModels.Market("BTC_JPY", "BTC_JPY") }));

    public Task<Call<RawPublicModels.GetMarketsRequest, IReadOnlyList<RawPublicModels.Market>>> MarketsCallAsync(
        RawPublicModels.GetMarketsRequest request,
        CancellationToken cancellationToken = default) =>
        GetMarketsCallAsync(request, cancellationToken);

    public Task<Call<RawPublicModels.GetChatsRequest, IReadOnlyList<RawPublicModels.Chat>>> GetChatsCallAsync(
        RawPublicModels.GetChatsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, (IReadOnlyList<RawPublicModels.Chat>)new[] { new RawPublicModels.Chat("n", "m", DateTimeOffset.UtcNow) }));

    public Task<Call<RawPublicModels.GetHealthRequest, RawPublicModels.HealthResponse>> GetHealthCallAsync(
        RawPublicModels.GetHealthRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.HealthResponse("NORMAL")));

    public Task<Call<RawPublicModels.GetBoardStateRequest, RawPublicModels.BoardStateResponse>> GetBoardStateCallAsync(
        RawPublicModels.GetBoardStateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.BoardStateResponse("NORMAL", "RUNNING", null)));

    public Task<Call<RawPublicModels.GetCorporateLeverageRequest, RawPublicModels.CorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        RawPublicModels.GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.CorporateLeverageResponse(
            CurrentMax: 7.7m,
            CurrentStartDate: DateTimeOffset.UtcNow,
            NextMax: 7.65m,
            NextStartDate: DateTimeOffset.UtcNow.AddDays(7))));

    public Task<Call<RawPublicModels.GetFundingRateRequest, RawPublicModels.FundingRateResponse>> GetFundingRateCallAsync(
        RawPublicModels.GetFundingRateRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(MakeOkCall(request, new RawPublicModels.FundingRateResponse(0m, DateTimeOffset.UtcNow)));

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
