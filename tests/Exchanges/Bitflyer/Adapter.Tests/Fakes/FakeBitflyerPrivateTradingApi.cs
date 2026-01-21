using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi
{
    private readonly RawPrivateModels.RawSendChildOrderResponse _response;
    private readonly IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse> _childOrders;
    private readonly Queue<IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>? _childOrderSnapshots;
    private readonly Exception? _exceptionToThrow;

    public string? LastBodyJson { get; private set; }
    public string? LastParentOrderBodyJson { get; private set; }
    public RawPrivateModels.CancelChildOrderRequest? LastCancelRequest { get; private set; }
    public RawPrivateModels.CancelParentOrderRequest? LastCancelParentOrderRequest { get; private set; }
    public RawPrivateModels.GetChildOrdersRequest? LastGetChildOrdersRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        RawPrivateModels.RawSendChildOrderResponse response,
        IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>? childOrders = null,
        IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>[]? snapshots = null,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _childOrders = childOrders ?? Array.Empty<RawPrivateModels.RawGetChildOrdersResponse>();
        _childOrderSnapshots = snapshots is null ? null : new Queue<IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>(
            snapshots);
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<Call<string, RawPrivateModels.RawSendChildOrderResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(MakeCall(bodyJson, _response));
    }

    public Task<Call<string, RawPrivateModels.RawSendParentOrderResponse>> SendParentOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        LastParentOrderBodyJson = bodyJson;
        return Task.FromResult(MakeCall(
            bodyJson,
            new RawPrivateModels.RawSendParentOrderResponse { ParentOrderAcceptanceId = "PARENT-1" }));
    }

    public Task<Call<RawPrivateModels.CancelChildOrderRequest, RawPrivateModels.RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
        RawPrivateModels.CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(MakeCall(request, new RawPrivateModels.RawCancelChildOrderResponse()));
    }

    public Task<Call<RawPrivateModels.CancelParentOrderRequest, RawPrivateModels.RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
        RawPrivateModels.CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
        return Task.FromResult(MakeCall(request, new RawPrivateModels.RawCancelParentOrderResponse()));
    }

    public Task<Call<RawPrivateModels.GetChildOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
        RawPrivateModels.GetChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        LastGetChildOrdersRequest = request;
        if (_childOrderSnapshots is not null && _childOrderSnapshots.Count > 0)
        {
            var snapshot = _childOrderSnapshots.Dequeue();
            return Task.FromResult(MakeCall(request, snapshot));
        }

        IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse> response;
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

    public Task<Call<RawPrivateModels.GetParentOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
        RawPrivateModels.GetParentOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse> response = Array.Empty<RawPrivateModels.RawGetParentOrdersResponse>();
        return Task.FromResult(MakeCall(request, response));
    }

    public Task<Call<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.RawGetParentOrderResponse>> GetParentOrderCallAsync(
        RawPrivateModels.GetParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(MakeCall(request, new RawPrivateModels.RawGetParentOrderResponse()));
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
