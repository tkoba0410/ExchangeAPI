using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly CreateChildOrderResponse _response;
    private readonly Exception? _exceptionToThrow;

    public RawRequests.CreateChildOrderRequest? LastRequest { get; private set; }
    public RawRequests.CancelChildOrderRequest? LastCancelRequest { get; private set; }
    public RawRequests.CancelAllChildOrdersRequest? LastCancelAllRequest { get; private set; }
    public RawRequests.CreateWithdrawalRequest? LastWithdrawRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        CreateChildOrderResponse response,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<Call<RawRequests.CreateChildOrderRequest, CreateChildOrderResponse>> CreateChildOrderAsync(
        RawRequests.CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        return Task.FromResult(MakeCall(request, _response));
    }

    public Task<Call<RawRequests.CancelChildOrderRequest, EmptyResponse>> CancelChildOrderAsync(
        RawRequests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(MakeCall(request, new EmptyResponse()));
    }

    public Task<Call<RawRequests.CancelAllChildOrdersRequest, EmptyResponse>> CancelAllChildOrdersAsync(
        RawRequests.CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelAllRequest = request;
        return Task.FromResult(MakeCall(request, new EmptyResponse()));
    }

    public Task<Call<RawRequests.CreateWithdrawalRequest, CreateWithdrawalResponse>> CreateWithdrawalAsync(
        RawRequests.CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        LastWithdrawRequest = request;
        return Task.FromResult(MakeCall(request, new CreateWithdrawalResponse { MessageId = "WITHDRAW" }));
    }

    private Call<TReq, TResponse> MakeCall<TReq, TResponse>(TReq request, TResponse response)
    {
        var meta = new CallMeta(
            Layer: "Raw",
            Component: "FakeBitflyerPrivateTradingApi",
            Tags: null,
            Children: null);

        if (_exceptionToThrow is null)
        {
            return new Call<TReq, TResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<TResponse>.Ok(response),
                Meta: meta);
        }

        if (_exceptionToThrow is ExchangeApi.Core.Contracts.Errors.ExchangeApiException ex)
        {
            var statusCode = ex.StatusCode.HasValue ? (int)ex.StatusCode.Value : (int?)null;
            var error = new CallError(CallErrorKind.Http, ex.Message, ex, statusCode);
            return new Call<TReq, TResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<TResponse>.Err(error),
                Meta: meta);
        }

        return new Call<TReq, TResponse>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Err(new CallError(CallErrorKind.Unknown, _exceptionToThrow.Message, _exceptionToThrow)),
            Meta: meta);
    }
}
