using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly BitflyerSendChildOrderResponse _response;

    public BitflyerSendChildOrderRequest? LastRequest { get; private set; }
    public BitflyerCancelChildOrderRequest? LastCancelRequest { get; private set; }
    public BitflyerCancelAllChildOrdersRequest? LastCancelAllRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(BitflyerSendChildOrderResponse response)
    {
        _response = response;
    }

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken cancellationToken = default)
    {
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
}
