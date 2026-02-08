using System;
using System.Collections.Generic;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Providers.Credentials;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Tests.Composition.Tests.Credentials;

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
        var result = composite.Get(AccountId.ParseOrThrow("default"));

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
        var ex = Assert.Throws<InvalidOperationException>(() => composite.Get(AccountId.ParseOrThrow("default")));
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

        public ApiCredentials Get(AccountId accountId) => _creds;
    }

    private sealed class ThrowingProvider : IApiCredentialProvider
    {
        public ApiCredentials Get(AccountId accountId)
        {
            throw new InvalidOperationException("fail");
        }
    }

    private sealed class EmptyProvider : IApiCredentialProvider
    {
        public ApiCredentials Get(AccountId accountId)
        {
            return new ApiCredentials(string.Empty, string.Empty);
        }
    }
}
