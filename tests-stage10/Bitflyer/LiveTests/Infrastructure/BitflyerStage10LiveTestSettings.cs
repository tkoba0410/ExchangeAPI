using ExchangeApi.Composition.Providers.Credentials;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Stage10.Bitflyer.Vocabulary;

namespace ExchangeApi.Tests.Stage10.Bitflyer.LiveTests.Infrastructure;

internal static class BitflyerStage10LiveTestSettings
{
    public const string LiveEnabledEnvironmentVariable = "BITFLYER_STAGE10_LIVE";
    public const string WriteEnabledEnvironmentVariable = "BITFLYER_STAGE10_LIVE_ALLOW_WRITE";
    public const string ApiBaseUriEnvironmentVariable = "BITFLYER_API_BASE_URI";
    public const string ApiKeyEnvironmentVariable = "BITFLYER_API_KEY";
    public const string ApiSecretEnvironmentVariable = "BITFLYER_API_SECRET";
    public const string AccountIdEnvironmentVariable = "EXCHANGEAPI_BITFLYER_LIVE_ACCOUNT_ID";
    public const string CredentialFilePathEnvironmentVariable = "CREDENTIAL_FILE_PATH";
    public const string AgeSecretKeyPathEnvironmentVariable = "AGE_SECRET_KEY_PATH";
    public const string DefaultProductCode = ProductCodes.Default;
    public const decimal DefaultWriteSize = 0.001m;

    private static readonly string DefaultCredentialFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config",
        "exchangeapi",
        "secrets",
        "credentials.enc.json");

    private static readonly string DefaultAgeSecretKeyPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        ".config",
        "exchangeapi",
        "keys",
        "age.key");

    public static string? GetPublicSkipReason() =>
        IsTruthy(Environment.GetEnvironmentVariable(LiveEnabledEnvironmentVariable))
            ? null
            : $"Set {LiveEnabledEnvironmentVariable}=1 to enable Stage10 bitFlyer live tests.";

    public static string? GetPrivateSkipReason()
    {
        var publicSkipReason = GetPublicSkipReason();
        if (publicSkipReason is not null)
        {
            return publicSkipReason;
        }

        return HasAuthenticatedCredentialSource()
            ? null
            : $"Set {ApiKeyEnvironmentVariable}/{ApiSecretEnvironmentVariable}, or provide {CredentialFilePathEnvironmentVariable}/{AgeSecretKeyPathEnvironmentVariable}, to enable authenticated Stage10 live tests.";
    }

    public static string? GetWriteSkipReason()
    {
        var privateSkipReason = GetPrivateSkipReason();
        if (privateSkipReason is not null)
        {
            return privateSkipReason;
        }

        return IsTruthy(Environment.GetEnvironmentVariable(WriteEnabledEnvironmentVariable))
            ? null
            : $"Set {WriteEnabledEnvironmentVariable}=1 to enable Stage10 bitFlyer write live tests.";
    }

    public static Uri? ResolveBaseUri()
    {
        var configured = Environment.GetEnvironmentVariable(ApiBaseUriEnvironmentVariable);
        return string.IsNullOrWhiteSpace(configured) ? null : new Uri(configured);
    }

    public static BitflyerApiCredentials GetCredentials()
    {
        var directApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
        var directApiSecret = Environment.GetEnvironmentVariable(ApiSecretEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(directApiKey) &&
            !string.IsNullOrWhiteSpace(directApiSecret))
        {
            return new BitflyerApiCredentials
            {
                ApiKey = directApiKey.Trim(),
                ApiSecret = directApiSecret.Trim(),
            };
        }

        var (credentialFilePath, ageSecretKeyPath) = ResolveCredentialPaths();
        if (credentialFilePath is null || ageSecretKeyPath is null)
        {
            throw new InvalidOperationException(
                $"Set {ApiKeyEnvironmentVariable}/{ApiSecretEnvironmentVariable}, or provide {CredentialFilePathEnvironmentVariable}/{AgeSecretKeyPathEnvironmentVariable}, before running this Stage10 bitFlyer live test.");
        }

        var provider = new AgeEncryptedFileApiCredentialProvider(
            credentialFilePath,
            "bitflyer",
            ageSecretKeyPath);

        var credentials = provider.Get(AccountId.ParseOrThrow(GetOptional(AccountIdEnvironmentVariable, "default")));
        return new BitflyerApiCredentials
        {
            ApiKey = credentials.ApiKey,
            ApiSecret = credentials.ApiSecret,
        };
    }

    public static decimal ResolveSafeWriteSize() => DefaultWriteSize;

    private static bool IsTruthy(string? value) =>
        value is not null &&
        (value.Equals("1", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("true", StringComparison.OrdinalIgnoreCase) ||
         value.Equals("yes", StringComparison.OrdinalIgnoreCase));

    private static bool HasAuthenticatedCredentialSource()
    {
        var directApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
        var directApiSecret = Environment.GetEnvironmentVariable(ApiSecretEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(directApiKey) &&
            !string.IsNullOrWhiteSpace(directApiSecret))
        {
            return true;
        }

        var (credentialFilePath, ageSecretKeyPath) = ResolveCredentialPaths();
        return credentialFilePath is not null && ageSecretKeyPath is not null;
    }

    private static (string? CredentialFilePath, string? AgeSecretKeyPath) ResolveCredentialPaths()
    {
        var credentialFilePath = Environment.GetEnvironmentVariable(CredentialFilePathEnvironmentVariable);
        var ageSecretKeyPath = Environment.GetEnvironmentVariable(AgeSecretKeyPathEnvironmentVariable);

        if (!string.IsNullOrWhiteSpace(credentialFilePath) ||
            !string.IsNullOrWhiteSpace(ageSecretKeyPath))
        {
            return (
                NormalizeExistingPath(credentialFilePath),
                NormalizeExistingPath(ageSecretKeyPath));
        }

        return (
            File.Exists(DefaultCredentialFilePath) ? DefaultCredentialFilePath : null,
            File.Exists(DefaultAgeSecretKeyPath) ? DefaultAgeSecretKeyPath : null);
    }

    private static string GetOptional(string envName, string fallback)
    {
        var value = Environment.GetEnvironmentVariable(envName);
        return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
    }

    private static string? NormalizeExistingPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var trimmed = path.Trim();
        return File.Exists(trimmed) ? trimmed : null;
    }
}
