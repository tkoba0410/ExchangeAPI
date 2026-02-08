using System;
using ExchangeApi.Composition.Providers.Credentials;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Tests.Composition.Tests.Credentials;

public class EnvironmentVariableApiCredentialProvider_Tests
{
    [Fact]
    public void Get_ReturnsCredentials_WhenBothVariablesExist()
    {
        // Arrange
        var provider = new EnvironmentVariableApiCredentialProvider("bitflyer");
        var account = AccountId.ParseOrThrow("default");

        var apiKeyName = "BITFLYER_DEFAULT_API_KEY";
        var apiSecretName = "BITFLYER_DEFAULT_API_SECRET";

        Environment.SetEnvironmentVariable(apiKeyName, "key-123");
        Environment.SetEnvironmentVariable(apiSecretName, "secret-456");

        try
        {
            // Act
            var creds = provider.Get(account);

            // Assert
            Assert.Equal("key-123", creds.ApiKey);
            Assert.Equal("secret-456", creds.ApiSecret);
        }
        finally
        {
            Environment.SetEnvironmentVariable(apiKeyName, null);
            Environment.SetEnvironmentVariable(apiSecretName, null);
        }
    }

    [Fact]
    public void Get_Throws_WhenKeyMissing()
    {
        // Arrange
        var provider = new EnvironmentVariableApiCredentialProvider("bitflyer");
        var account = AccountId.ParseOrThrow("trading");

        var apiKeyName = "BITFLYER_TRADING_API_KEY";
        var apiSecretName = "BITFLYER_TRADING_API_SECRET";

        Environment.SetEnvironmentVariable(apiKeyName, null);
        Environment.SetEnvironmentVariable(apiSecretName, "secret-only");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => provider.Get(account));

        Environment.SetEnvironmentVariable(apiSecretName, null);
    }
}
