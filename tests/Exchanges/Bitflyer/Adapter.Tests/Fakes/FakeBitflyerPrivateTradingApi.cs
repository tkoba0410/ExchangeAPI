using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi
{
    private readonly RawPrivateDtos.SendChildOrderResponse _response;
    private readonly IReadOnlyList<RawPrivateDtos.GetChildOrdersItem> _childOrders;
    private readonly Queue<IReadOnlyList<RawPrivateDtos.GetChildOrdersItem>>? _childOrderSnapshots;
    private readonly Exception? _exceptionToThrow;

    public string? LastBodyJson { get; private set; }
    public string? LastParentOrderBodyJson { get; private set; }
    public RawPrivateRequests.CancelChildOrderRequest? LastCancelRequest { get; private set; }
    public RawPrivateRequests.CancelParentOrderRequest? LastCancelParentOrderRequest { get; private set; }
    public RawPrivateRequests.GetChildOrdersRequest? LastGetChildOrdersRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        RawPrivateDtos.SendChildOrderResponse response,
        IReadOnlyList<RawPrivateDtos.GetChildOrdersItem>? childOrders = null,
        IReadOnlyList<RawPrivateDtos.GetChildOrdersItem>[]? snapshots = null,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _childOrders = childOrders ?? Array.Empty<RawPrivateDtos.GetChildOrdersItem>();
        _childOrderSnapshots = snapshots is null ? null : new Queue<IReadOnlyList<RawPrivateDtos.GetChildOrdersItem>>(
            snapshots);
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<Call<RawPrivateRequests.SendChildOrderRequest, RawPrivateDtos.SendChildOrderResponse>> SendChildOrderCallAsync(
        RawPrivateRequests.SendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastBodyJson = JsonSerializer.Serialize(request);
        return Task.FromResult(MakeCall(request, _response));
    }

    public Task<Call<RawPrivateRequests.SendParentOrderRequest, RawPrivateDtos.SendParentOrderResponse>> SendParentOrderCallAsync(
        RawPrivateRequests.SendParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastParentOrderBodyJson = JsonSerializer.Serialize(request);
        return Task.FromResult(MakeCall(
            request,
            new RawPrivateDtos.SendParentOrderResponse { ParentOrderAcceptanceId = "PARENT-1" }));
    }

    public Task<Call<RawPrivateRequests.CancelChildOrderRequest, RawPrivateDtos.CancelChildOrderResponse>> CancelChildOrderCallAsync(
        RawPrivateRequests.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(MakeCall(request, new RawPrivateDtos.CancelChildOrderResponse()));
    }

    public Task<Call<RawPrivateRequests.CancelParentOrderRequest, RawPrivateDtos.CancelParentOrderResponse>> CancelParentOrderCallAsync(
        RawPrivateRequests.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
        return Task.FromResult(MakeCall(request, new RawPrivateDtos.CancelParentOrderResponse()));
    }

    public Task<Call<RawPrivateRequests.GetChildOrdersRequest, RawPrivateDtos.GetChildOrdersResponse>> GetChildOrdersCallAsync(
        RawPrivateRequests.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        LastGetChildOrdersRequest = request;
        if (_childOrderSnapshots is not null && _childOrderSnapshots.Count > 0)
        {
            var snapshot = _childOrderSnapshots.Dequeue();
            var snapshotResponse = new RawPrivateDtos.GetChildOrdersResponse();
            snapshotResponse.AddRange(snapshot);
            return Task.FromResult(MakeCall(request, snapshotResponse));
        }

        IReadOnlyList<RawPrivateDtos.GetChildOrdersItem> responseItems;
        if (request.ChildOrderAcceptanceId is { IsEmpty: false })
        {
            responseItems = _childOrders
                .Where(o => o.ChildOrderAcceptanceId == request.ChildOrderAcceptanceId.Value.Value)
                .ToArray();
        }
        else if (request.ChildOrderId is { IsEmpty: false })
        {
            responseItems = _childOrders
                .Where(o => o.ChildOrderId == request.ChildOrderId.Value.Value)
                .ToArray();
        }
        else
        {
            responseItems = _childOrders;
        }
        var response = new RawPrivateDtos.GetChildOrdersResponse();
        response.AddRange(responseItems);
        return Task.FromResult(MakeCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetParentOrdersRequest, RawPrivateDtos.GetParentOrdersResponse>> GetParentOrdersCallAsync(
        RawPrivateRequests.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new RawPrivateDtos.GetParentOrdersResponse();
        return Task.FromResult(MakeCall(request, response));
    }

    public Task<Call<RawPrivateRequests.GetParentOrderRequest, RawPrivateDtos.GetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateRequests.GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(MakeCall(request, new RawPrivateDtos.GetParentOrderResponse()));
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

        if (_exceptionToThrow is ExchangeApi.Primitives.Errors.ExchangeApiException ex)
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
