using System;
using System.IO;
using ExchangeApi.Composition.Providers.Credentials;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Tests.Composition.Tests.Credentials;

public class AgeEncryptedFileApiCredentialProvider_Tests
{
    [Fact]
    public void Get_ReturnsCredentials_WhenSchemaIsValid()
    {
        var encryptedPath = Path.GetTempFileName();
        var keyPath = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": {
                "ApiKey": "key-1",
                "ApiSecret": "secret-1",
                "ExpiresAt": "2026-03-01T00:00:00Z",
                "Version": null,
                "UpdatedAt": "2026-02-19T00:00:00Z",
                "Comment": null
              }
            }
            """;

            var provider = new AgeEncryptedFileApiCredentialProvider(
                encryptedPath,
                "bitflyer",
                keyPath,
                decryptor: (_, _) => json);

            var credentials = provider.Get(AccountId.ParseOrThrow("default"));

            Assert.Equal("key-1", credentials.ApiKey);
            Assert.Equal("secret-1", credentials.ApiSecret);
            Assert.Equal(DateTimeOffset.Parse("2026-03-01T00:00:00Z"), credentials.ExpiresAt);
        }
        finally
        {
            File.Delete(encryptedPath);
            File.Delete(keyPath);
        }
    }

    [Fact]
    public void Constructor_Throws_WhenVersionPropertyMissing()
    {
        var encryptedPath = Path.GetTempFileName();
        var keyPath = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": {
                "ApiKey": "key-1",
                "ApiSecret": "secret-1",
                "ExpiresAt": null,
                "UpdatedAt": null,
                "Comment": null
              }
            }
            """;

            var exception = Assert.Throws<InvalidOperationException>(() =>
                new AgeEncryptedFileApiCredentialProvider(
                    encryptedPath,
                    "bitflyer",
                    keyPath,
                    decryptor: (_, _) => json));

            Assert.Contains("Version", exception.Message);
        }
        finally
        {
            File.Delete(encryptedPath);
            File.Delete(keyPath);
        }
    }

    [Fact]
    public void Constructor_Throws_WhenUpdatedAtIsInvalid()
    {
        var encryptedPath = Path.GetTempFileName();
        var keyPath = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": {
                "ApiKey": "key-1",
                "ApiSecret": "secret-1",
                "ExpiresAt": null,
                "Version": null,
                "UpdatedAt": "not-a-date",
                "Comment": null
              }
            }
            """;

            var exception = Assert.Throws<InvalidOperationException>(() =>
                new AgeEncryptedFileApiCredentialProvider(
                    encryptedPath,
                    "bitflyer",
                    keyPath,
                    decryptor: (_, _) => json));

            Assert.Contains("UpdatedAt", exception.Message);
        }
        finally
        {
            File.Delete(encryptedPath);
            File.Delete(keyPath);
        }
    }

    [Fact]
    public void Constructor_Throws_WhenExpiresAtIsInvalid()
    {
        var encryptedPath = Path.GetTempFileName();
        var keyPath = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": {
                "ApiKey": "key-1",
                "ApiSecret": "secret-1",
                "ExpiresAt": "invalid-date",
                "Version": null,
                "UpdatedAt": null,
                "Comment": null
              }
            }
            """;

            var exception = Assert.Throws<InvalidOperationException>(() =>
                new AgeEncryptedFileApiCredentialProvider(
                    encryptedPath,
                    "bitflyer",
                    keyPath,
                    decryptor: (_, _) => json));

            Assert.Contains("ExpiresAt", exception.Message);
        }
        finally
        {
            File.Delete(encryptedPath);
            File.Delete(keyPath);
        }
    }

    [Fact]
    public void Constructor_Throws_WhenUpdatedAtIsNotUtcZFormat()
    {
        var encryptedPath = Path.GetTempFileName();
        var keyPath = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": {
                "ApiKey": "key-1",
                "ApiSecret": "secret-1",
                "ExpiresAt": null,
                "Version": null,
                "UpdatedAt": "2026-02-19T09:00:00+09:00",
                "Comment": null
              }
            }
            """;

            var exception = Assert.Throws<InvalidOperationException>(() =>
                new AgeEncryptedFileApiCredentialProvider(
                    encryptedPath,
                    "bitflyer",
                    keyPath,
                    decryptor: (_, _) => json));

            Assert.Contains("UpdatedAt", exception.Message);
        }
        finally
        {
            File.Delete(encryptedPath);
            File.Delete(keyPath);
        }
    }

    [Fact]
    public void Get_Throws_WhenAccountCredentialsMissing()
    {
        var encryptedPath = Path.GetTempFileName();
        var keyPath = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": {
                "ApiKey": "key-1",
                "ApiSecret": "secret-1",
                "ExpiresAt": null,
                "Version": null,
                "UpdatedAt": null,
                "Comment": null
              }
            }
            """;

            var provider = new AgeEncryptedFileApiCredentialProvider(
                encryptedPath,
                "bitflyer",
                keyPath,
                decryptor: (_, _) => json);

            var exception = Assert.Throws<InvalidOperationException>(() =>
                provider.Get(AccountId.ParseOrThrow("trading")));

            Assert.Contains("CRED_NOT_FOUND", exception.Message);
        }
        finally
        {
            File.Delete(encryptedPath);
            File.Delete(keyPath);
        }
    }
}
