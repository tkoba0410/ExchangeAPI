using System;
using Common.Interfaces;
using Common.Dtos;
using Common.Enums;
using Core.Transport.Policy;
using Exchange.Bitflyer.Abstract.Factory;
using Xunit;

namespace Exchange.Bitflyer.Tests;

    public class BitflyerClientFactory_Tests
    {
        [Fact]
        public void Create_WithProvider_DelegatesToProviderAndReturnsClient()
        {
        // Arrange
        var provider = new FakeProvider(new ApiCredentials("key-1", "secret-1"));

        // Act
        var client = BitflyerClientFactory.Create(provider, "bitflyer", "default");

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Create_WithProvider_NullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            BitflyerClientFactory.Create(provider: null!, "bitflyer", "default"));
    }

    [Fact]
    public void Create_WithOptions_Succeeds()
    {
        var options = new BitflyerClientOptions
        {
            PolicyOptions = new HttpPolicyOptions { RequestsPerSecond = 10 }
        };

        var client = BitflyerClientFactory.Create("key-1", "secret-1", options);

        Assert.NotNull(client);
    }

    private sealed class FakeProvider : IApiCredentialProvider
    {
        private readonly ApiCredentials _creds;

        public FakeProvider(ApiCredentials creds)
        {
            _creds = creds;
        }

        public ApiCredentials Get(string exchangeId, string accountId) => _creds;
    }

}
