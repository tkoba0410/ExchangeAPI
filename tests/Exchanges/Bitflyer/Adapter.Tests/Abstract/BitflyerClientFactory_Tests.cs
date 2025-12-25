using System;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

    public class BitflyerClientFactory_Tests
    {
        [Fact]
        public void Create_WithProvider_DelegatesToProviderAndReturnsClient()
        {
        // Arrange
        var provider = new FakeProvider(new ApiCredentials("key-1", "secret-1"));

        // Act
        var client = BitflyerClientFactory.Create(provider, ExchangeCode.Bitflyer, "default");

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Create_WithProvider_NullProvider_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            BitflyerClientFactory.Create(provider: null!, ExchangeCode.Bitflyer, "default"));
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

        public ApiCredentials Get(ExchangeCode exchange, string accountId) => _creds;
    }

}
