using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi
{
    private readonly RawPrivateDtos.RawSendChildOrderResponse _response;
    private readonly IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse> _childOrders;
    private readonly Queue<IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>>? _childOrderSnapshots;
    private readonly Exception? _exceptionToThrow;

    public string? LastBodyJson { get; private set; }
    public string? LastParentOrderBodyJson { get; private set; }
    public RawPrivateRequests.CancelChildOrderRequest? LastCancelRequest { get; private set; }
    public RawPrivateRequests.CancelParentOrderRequest? LastCancelParentOrderRequest { get; private set; }
    public RawPrivateRequests.GetChildOrdersRequest? LastGetChildOrdersRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        RawPrivateDtos.RawSendChildOrderResponse response,
        IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>? childOrders = null,
        IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>[]? snapshots = null,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _childOrders = childOrders ?? Array.Empty<RawPrivateDtos.RawGetChildOrdersResponse>();
        _childOrderSnapshots = snapshots is null ? null : new Queue<IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>>(
            snapshots);
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<Call<RawPrivateRequests.CreateChildOrderRequest, RawPrivateDtos.RawSendChildOrderResponse>> SendChildOrderCallAsync(
        RawPrivateRequests.CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastBodyJson = JsonSerializer.Serialize(request);
        return Task.FromResult(MakeCall(request, _response));
    }

    public Task<Call<RawPrivateRequests.CreateParentOrderRequest, RawPrivateDtos.RawSendParentOrderResponse>> SendParentOrderCallAsync(
        RawPrivateRequests.CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastParentOrderBodyJson = JsonSerializer.Serialize(request);
        return Task.FromResult(MakeCall(
            request,
            new RawPrivateDtos.RawSendParentOrderResponse { ParentOrderAcceptanceId = "PARENT-1" }));
    }

    public Task<Call<RawPrivateRequests.CancelChildOrderRequest, RawPrivateDtos.RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        RawPrivateRequests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(MakeCall(request, new RawPrivateDtos.RawCancelChildOrderResponse()));
    }

    public Task<Call<RawPrivateRequests.CancelParentOrderRequest, RawPrivateDtos.RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        RawPrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
        return Task.FromResult(MakeCall(request, new RawPrivateDtos.RawCancelParentOrderResponse()));
    }

    public Task<Call<RawPrivateRequests.GetChildOrdersRequest, IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        RawPrivateRequests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        LastGetChildOrdersRequest = request;
        if (_childOrderSnapshots is not null && _childOrderSnapshots.Count > 0)
        {
            var snapshot = _childOrderSnapshots.Dequeue();
            return Task.FromResult(MakeCall(request, snapshot));
        }

        IReadOnlyList<RawPrivateDtos.RawGetChildOrdersResponse> response;
        if (!string.IsNullOrWhiteSpace(request.ChildOrderAcceptanceId))
        {
            response = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == request.ChildOrderAcceptanceId)
                .ToArray();
        }
        else if (!string.IsNullOrWhiteSpace(request.ChildOrderId))
        {
            response = _childOrders
                .Where(o => o.ChildOrderId == request.ChildOrderId)
                .ToArray();
        }
        else
        {
            response = _childOrders;
        }
        return Task.FromResult(MakeCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetParentOrdersRequest, IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        RawPrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse> response = Array.Empty<RawPrivateDtos.RawGetParentOrdersResponse>();
        return Task.FromResult(MakeCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetParentOrderRequest, RawPrivateDtos.RawGetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(MakeCall(request, new RawPrivateDtos.RawGetParentOrderResponse()));
    }

    private Call<TReq, TResponse> MakeCall<TReq, TResponse>(TReq request, TResponse response)
    {
        var meta = CallMeta.CreateInternal("Raw", "FakeBitflyerPrivateTradingApi");

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

        if (_exceptionToThrow is ExchangeApi.Contracts.Common.Errors.ExchangeApiException ex)
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
