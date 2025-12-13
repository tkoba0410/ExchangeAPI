using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Raw;
using Common.Transport.Protocol;
using Xunit;

namespace Exchange.Bitflyer.Tests;

public sealed class BitflyerPrivateApi_SendChildOrder_Tests
{
    [Fact]
    public async Task SendChildOrderAsync_CallsRestClientPostWithCorrectPath()
    {
        var fakeRest = new FakeRestClient();
        var api = new BitflyerPrivateTradingApi(fakeRest);

        var request = new BitflyerSendChildOrderRequest
        {
            ProductCode = ProductCode.BtcJpy,
            ChildOrderType = ChildOrderType.Market,
            Side = Side.Buy,
            Size = 0.01m,
        };

        await api.SendChildOrderAsync(request, CancellationToken.None);

        Assert.Equal("/v1/me/sendchildorder", fakeRest.LastPath);
        Assert.Equal(request, fakeRest.LastBody);
    }

    private sealed class FakeRestClient : IRestClient
    {
        public string? LastPath { get; private set; }
        public object? LastBody { get; private set; }

        public Task<TResponse> GetAsync<TResponse>(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default)
        {
            LastPath = path;
            LastBody = body;
            return Task.FromResult(default(TResponse)!);
        }
    }
}
