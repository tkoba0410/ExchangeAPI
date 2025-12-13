using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;

namespace Exchange.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly BitflyerSendChildOrderResponse _response;
    private readonly Exception? _exceptionToThrow;

    public BitflyerSendChildOrderRequest? LastRequest { get; private set; }
    public BitflyerCancelChildOrderRequest? LastCancelRequest { get; private set; }
    public BitflyerCancelAllChildOrdersRequest? LastCancelAllRequest { get; private set; }
    public BitflyerSendParentOrderRequest? LastParentOrderRequest { get; private set; }
    public BitflyerCancelParentOrderRequest? LastCancelParentOrderRequest { get; private set; }
    public BitflyerWithdrawRequest? LastWithdrawRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        BitflyerSendChildOrderResponse response,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (_exceptionToThrow is not null)
        {
            throw _exceptionToThrow;
        }

        LastRequest = request;
        return Task.FromResult(_response);
    }

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(BitflyerCancelChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(new BitflyerEmptyResponse());
    }

    public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(BitflyerCancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelAllRequest = request;
        return Task.FromResult(new BitflyerEmptyResponse());
    }

    public Task<BitflyerSendParentOrderResponse> SendParentOrderAsync(BitflyerSendParentOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastParentOrderRequest = request;
        return Task.FromResult(new BitflyerSendParentOrderResponse { ParentOrderAcceptanceId = "PARENT" });
    }

    public Task<BitflyerEmptyResponse> CancelParentOrderAsync(BitflyerCancelParentOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelParentOrderRequest = request;
        return Task.FromResult(new BitflyerEmptyResponse());
    }

    public Task<BitflyerWithdrawResponse> WithdrawAsync(BitflyerWithdrawRequest request, CancellationToken cancellationToken = default)
    {
        LastWithdrawRequest = request;
        return Task.FromResult(new BitflyerWithdrawResponse { MessageId = "WITHDRAW" });
    }
}
