using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Extensions;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using ExchangeApi.Exchanges.Bitflyer.Raw;
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
        Assert.NotNull(wire);
    }

    private sealed class FakeRestClient : IRestClient
    {
        public Task<TResponse> GetAsync<TResponse>(string path, IReadOnlyDictionary<string, string?>? query = null, CancellationToken cancellationToken = default) =>
            throw new System.NotImplementedException();

        public Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken cancellationToken = default) =>
            throw new System.NotImplementedException();
    }
}
