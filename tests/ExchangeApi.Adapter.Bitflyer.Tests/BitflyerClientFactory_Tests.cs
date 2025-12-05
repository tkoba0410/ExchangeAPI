using System;
using ExchangeApi.Core.Contracts;
using ExchangeApi.Core.Dtos;
using ExchangeApi.Adapter.Bitflyer;
using Xunit;

namespace ExchangeApi.Adapter.Bitflyer.Tests;

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
