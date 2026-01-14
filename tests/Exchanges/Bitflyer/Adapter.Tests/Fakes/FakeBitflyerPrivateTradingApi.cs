using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Contracts.Common.CallCommon;
using RawRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly CreateChildOrderResponse _response;
    private readonly Exception? _exceptionToThrow;

    public string? LastBodyJson { get; private set; }
    public string? LastParentOrderBodyJson { get; private set; }
    public RawRequests.CancelChildOrderRequest? LastCancelRequest { get; private set; }
    public RawRequests.CancelParentOrderRequest? LastCancelParentOrderRequest { get; private set; }
    public RawRequests.CancelAllChildOrdersRequest? LastCancelAllRequest { get; private set; }
    public RawRequests.CreateWithdrawalRequest? LastWithdrawRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        CreateChildOrderResponse response,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<Call<string, CreateChildOrderResponse>> CreateChildOrderAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(MakeCall(bodyJson, _response));
    }

    public Task<Call<string, CreateParentOrderResponse>> CreateParentOrderAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        LastParentOrderBodyJson = bodyJson;
        return Task.FromResult(MakeCall(
            bodyJson,
            new CreateParentOrderResponse { ParentOrderAcceptanceId = "PARENT-1" }));
    }

    public Task<Call<RawRequests.CancelChildOrderRequest, EmptyResponse>> CancelChildOrderAsync(
        RawRequests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(MakeCall(request, new EmptyResponse()));
    }

    public Task<Call<RawRequests.CancelParentOrderRequest, EmptyResponse>> CancelParentOrderAsync(
        RawRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
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

        if (_exceptionToThrow is ExchangeApi.Contracts.Errors.ExchangeApiException ex)
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
