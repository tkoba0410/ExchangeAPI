using System;
using System.Collections.Generic;
using ExchangeApi.Abstractions.Contracts;
using ExchangeApi.Abstractions.Dtos;
using ExchangeApi.Orchestration.Credentials;

namespace ExchangeApi.Orchestration.Tests.Credentials;

public class CompositeCredentialProvider_Tests
{
    [Fact]
    public void Get_ReturnsFirstSuccessfulProvider()
    {
        // Arrange
        var expected = new ApiCredentials("key", "secret");
        var providers = new IApiCredentialProvider[]
        {
            new ThrowingProvider(),
            new FixedProvider(expected),
        };

        var composite = new CompositeCredentialProvider(providers);

        // Act
        var result = composite.Get("bitflyer", "default");

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Get_Throws_WhenAllProvidersFail()
    {
        // Arrange
        var providers = new IApiCredentialProvider[]
        {
            new ThrowingProvider(),
            new EmptyProvider(),
        };

        var composite = new CompositeCredentialProvider(providers);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => composite.Get("bitflyer", "default"));
    }

    private sealed class FixedProvider : IApiCredentialProvider
    {
        private readonly ApiCredentials _creds;

        public FixedProvider(ApiCredentials creds)
        {
            _creds = creds;
        }

        public ApiCredentials Get(string exchangeId, string accountId) => _creds;
    }

    private sealed class ThrowingProvider : IApiCredentialProvider
    {
        public ApiCredentials Get(string exchangeId, string accountId)
        {
            throw new InvalidOperationException("fail");
        }
    }

    private sealed class EmptyProvider : IApiCredentialProvider
    {
        public ApiCredentials Get(string exchangeId, string accountId)
        {
            return new ApiCredentials(string.Empty, string.Empty);
        }
    }
}
