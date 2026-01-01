using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly CreateChildOrderResponse _response;
    private readonly Exception? _exceptionToThrow;
    private static readonly BitflyerRawRequest DefaultRequest =
        new BitflyerRawRequest("test", new Dictionary<string, string?>());

    public CreateChildOrderRequest? LastRequest { get; private set; }
    public CancelChildOrderRequest? LastCancelRequest { get; private set; }
    public CancelAllChildOrdersRequest? LastCancelAllRequest { get; private set; }
    public CreateParentOrderRequest? LastParentOrderRequest { get; private set; }
    public CancelParentOrderRequest? LastCancelParentOrderRequest { get; private set; }
    public CreateWithdrawalRequest? LastWithdrawRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        CreateChildOrderResponse response,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<CreateChildOrderResponse> CreateChildOrderAsync(CreateChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (_exceptionToThrow is not null)
        {
            throw _exceptionToThrow;
        }

        LastRequest = request;
        return Task.FromResult(_response);
    }

    public Task<EmptyResponse> CancelChildOrderAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(new EmptyResponse());
    }

    public Task<EmptyResponse> CancelAllChildOrdersAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelAllRequest = request;
        return Task.FromResult(new EmptyResponse());
    }

    public Task<CreateParentOrderResponse> CreateParentOrderAsync(CreateParentOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastParentOrderRequest = request;
        return Task.FromResult(new CreateParentOrderResponse { ParentOrderAcceptanceId = "PARENT" });
    }

    public Task<EmptyResponse> CancelParentOrderAsync(CancelParentOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
        return Task.FromResult(new EmptyResponse());
    }

    public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(CreateWithdrawalRequest request, CancellationToken cancellationToken = default)
    {
        LastWithdrawRequest = request;
        return Task.FromResult(new CreateWithdrawalResponse { MessageId = "WITHDRAW" });
    }

    public Task<BitflyerRawCall<CreateChildOrderResponse, JsonElement>> CreateChildOrderCallAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        return Task.FromResult(MakeCall(_response));
    }

    public Task<BitflyerRawCall<EmptyResponse, JsonElement>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(MakeCall(new EmptyResponse()));
    }

    public Task<BitflyerRawCall<EmptyResponse, JsonElement>> CancelAllChildOrdersCallAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelAllRequest = request;
        return Task.FromResult(MakeCall(new EmptyResponse()));
    }

    public Task<BitflyerRawCall<CreateParentOrderResponse, JsonElement>> CreateParentOrderCallAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastParentOrderRequest = request;
        return Task.FromResult(MakeCall(new CreateParentOrderResponse { ParentOrderAcceptanceId = "PARENT" }));
    }

    public Task<BitflyerRawCall<EmptyResponse, JsonElement>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
        return Task.FromResult(MakeCall(new EmptyResponse()));
    }

    public Task<BitflyerRawCall<CreateWithdrawalResponse, JsonElement>> CreateWithdrawalCallAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default)
    {
        LastWithdrawRequest = request;
        return Task.FromResult(MakeCall(new CreateWithdrawalResponse { MessageId = "WITHDRAW" }));
    }

    private BitflyerRawCall<TResponse, JsonElement> MakeCall<TResponse>(TResponse response)
    {
        if (_exceptionToThrow is null)
        {
            return MakeOkCall(response);
        }

        if (_exceptionToThrow is ExchangeApi.Core.Contracts.Errors.ExchangeApiException ex)
        {
            return MakeErrCall<TResponse>(ex);
        }

        return MakeErrCall<TResponse>(new ExchangeApi.Core.Contracts.Errors.ExchangeApiException(_exceptionToThrow.Message));
    }

    private static BitflyerRawCall<TResponse, JsonElement> MakeOkCall<TResponse>(TResponse response) =>
        new(
            DefaultRequest,
            new Ok<TResponse, JsonElement>(response, 200),
            new CallMeta(System.DateTimeOffset.UtcNow, System.TimeSpan.Zero, null));

    private static BitflyerRawCall<TResponse, JsonElement> MakeErrCall<TResponse>(ExchangeApi.Core.Contracts.Errors.ExchangeApiException ex)
    {
        var statusCode = ex.StatusCode.HasValue ? (int)ex.StatusCode.Value : 400;
        var error = JsonDocument.Parse("{}").RootElement;
        return new BitflyerRawCall<TResponse, JsonElement>(
            DefaultRequest,
            new Err<TResponse, JsonElement>(error, statusCode),
            new CallMeta(System.DateTimeOffset.UtcNow, System.TimeSpan.Zero, null));
    }
}
