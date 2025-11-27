using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Bitflyer;
using ExchangeApi.Bitflyer.Models;

namespace ExchangeApi.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly BitflyerSendChildOrderResponse _response;

    public BitflyerSendChildOrderRequest? LastRequest { get; private set; }

    public FakeBitflyerPrivateTradingApi(BitflyerSendChildOrderResponse response)
    {
        _response = response;
    }

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        return Task.FromResult(_response);
    }
}
