using ExchangeApi.Optional.Credentials.Profiles;

namespace ExchangeApi.Optional.Credentials.Tests;

public sealed class CredentialProfileTests
{
    [Fact]
    public void Load_ReadsCanonicalCredentialProfile()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var profileFilePath = Path.Combine(tempDirectory.FullName, "credential-profile.json");
            File.WriteAllText(
                profileFilePath,
                """
                {
                  "version": 1,
                  "credentials": {
                    "bitflyer": {
                      "provider": "age-file",
                      "identityFilePath": "current/age-identity.txt",
                      "credentialsFilePath": "current/bitflyer.age"
                    }
                  }
                }
                """);

            var profile = CredentialProfileLoader.Load(profileFilePath);

            Assert.Equal(1, profile.Version);
            Assert.True(profile.Credentials.TryGetValue("bitflyer", out var bitflyer));
            Assert.Equal("age-file", bitflyer.Provider);
            Assert.Equal("current/age-identity.txt", bitflyer.IdentityFilePath);
            Assert.Equal("current/bitflyer.age", bitflyer.CredentialsFilePath);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Load_ReadsCTradeBotFlatCredentialSettings()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var profileFilePath = Path.Combine(tempDirectory.FullName, "cbot.settings.json");
            File.WriteAllText(
                profileFilePath,
                """
                {
                  "credentialsSource": "AgeFile",
                  "credentialsAgeFile": "./bitflyer-credentials.age",
                  "ageIdentityFile": "./age-identity.txt"
                }
                """);

            var profile = CredentialProfileLoader.Load(profileFilePath);

            Assert.True(profile.Credentials.TryGetValue("bitflyer", out var bitflyer));
            Assert.Equal("age-file", bitflyer.Provider);
            Assert.Equal("./age-identity.txt", bitflyer.IdentityFilePath);
            Assert.Equal("./bitflyer-credentials.age", bitflyer.CredentialsFilePath);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }
}
