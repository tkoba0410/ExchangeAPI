using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly CreateChildOrderResponse _response;
    private readonly Exception? _exceptionToThrow;

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
}
