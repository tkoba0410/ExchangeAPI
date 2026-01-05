using System;
using System.Collections.Generic;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Composition.Providers.Credentials;

namespace Composition.Tests.Credentials;

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
        var result = composite.Get(ExchangeCode.Bitflyer, "default");

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Get_Throws_WhenAllProvidersFail()
    {
        var providers = new IApiCredentialProvider[]
        {
            new ThrowingProvider(),
            new EmptyProvider(),
        };

        var composite = new CompositeCredentialProvider(providers);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => composite.Get(ExchangeCode.Bitflyer, "default"));
        Assert.Contains("ThrowingProvider", ex.Message);
        Assert.Contains("EmptyProvider", ex.Message);
    }

    private sealed class FixedProvider : IApiCredentialProvider
    {
        private readonly ApiCredentials _creds;

        public FixedProvider(ApiCredentials creds)
        {
            _creds = creds;
        }

        public ApiCredentials Get(ExchangeCode exchange, string accountId) => _creds;
    }

    private sealed class ThrowingProvider : IApiCredentialProvider
    {
        public ApiCredentials Get(ExchangeCode exchange, string accountId)
        {
            throw new InvalidOperationException("fail");
        }
    }

    private sealed class EmptyProvider : IApiCredentialProvider
    {
        public ApiCredentials Get(ExchangeCode exchange, string accountId)
        {
            return new ApiCredentials(string.Empty, string.Empty);
        }
    }
}
