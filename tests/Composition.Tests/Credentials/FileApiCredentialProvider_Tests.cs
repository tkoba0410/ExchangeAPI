using System;
using System.IO;
using System.Text.Json;
using ExchangeApi.Composition.Credentials;

namespace Composition.Tests.Credentials;

public class FileApiCredentialProvider_Tests
{
    [Fact]
    public void Get_ReturnsCredentialsFromFile()
    {
        var path = Path.GetTempFileName();
        try
        {
            var json = """
            {
              "bitflyer/default": { "ApiKey": "key1", "ApiSecret": "sec1" }
            }
            """;
            File.WriteAllText(path, json);

            var provider = new FileApiCredentialProvider(path);

            var creds = provider.Get("bitflyer", "default");

            Assert.Equal("key1", creds.ApiKey);
            Assert.Equal("sec1", creds.ApiSecret);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Get_Throws_WhenFileMissing()
    {
        var missingPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        Assert.Throws<FileNotFoundException>(() => new FileApiCredentialProvider(missingPath));
    }

    [Fact]
    public void Get_Throws_WhenInvalidJson()
    {
        var path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, "{ invalid json");
            Assert.Throws<JsonException>(() => new FileApiCredentialProvider(path));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
