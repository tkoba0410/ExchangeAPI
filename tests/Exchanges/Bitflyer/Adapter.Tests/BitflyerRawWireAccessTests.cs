using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Extensions;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Tests;

public sealed class BitflyerRawWireAccessTests
{
    [Fact]
    public void Raw_And_Wire_Are_Available_From_Client()
    {
        var client = BitflyerTestClientFactory.Create(new FakeRestClient());

        var raw = client.Raw<IBitflyerRawApi>();
        var wire = client.Wire<IBitflyerWireApi>();

        Assert.NotNull(raw);
        Assert.NotNull(raw.Trading);
        Assert.NotNull(wire);
        Assert.NotNull(wire.Trading);
    }

    [Fact]
    public async Task PublicClient_WireTrading_IsNotSupported()
    {
        var client = BitflyerClientFactory.CreatePublic();

        var wire = client.Wire<IBitflyerWireApi>();

        var request = new CreateChildOrderRequest
        {
            ProductCode = new RawProductCode("BTC_JPY"),
            ChildOrderType = ChildOrderType.Market,
            Side = Side.Buy,
            Size = 0.01m,
        };

        await Assert.ThrowsAsync<ExchangeFeatureNotSupportedException>(() =>
            wire.Trading.CreateChildOrderAsync(request));
    }

    private sealed class FakeRestClient : IRestClient
    {
        public Task<TResponse> GetAsync<TResponse>(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default) =>
            throw new System.NotImplementedException();

        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default) =>
            throw new System.NotImplementedException();
    }
}
