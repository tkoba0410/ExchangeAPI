using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Optional.Credentials.Profiles;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Adapters.McpServer.Configuration;

public static class BitflyerCredentialResolver
{
    public const int CanonicalVersion = 1;
    public const string CanonicalVenue = "bitflyer";
    public const string CredentialProfileOptionName = "credential-profile";
    public static readonly string DefaultCredentialProfilePath = CredentialProfileDefaults.DefaultProfilePath;
    public static readonly string AuthenticationRequirementText =
        $"--{CredentialProfileOptionName} / {CredentialProfileDefaults.DefaultProfilePath}";

    public static bool HasConfiguredCredentialsSource()
    {
        return HasConfiguredCredentialsSource(null);
    }

    public static bool HasConfiguredCredentialsSource(string? credentialProfilePath)
    {
        var profilePath = ResolveProfilePath(credentialProfilePath);
        if (!File.Exists(profilePath))
        {
            return false;
        }

        try
        {
            var profile = CredentialProfileLoader.Load(profilePath);
            _ = CredentialProfileProviderFactory.Create(profile, ExchangeVenue.Bitflyer, profilePath);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or System.Text.Json.JsonException)
        {
            return false;
        }
    }

    public static BitflyerCredentialResolution Resolve()
    {
        return Resolve(null);
    }

    public static BitflyerCredentialResolution Resolve(string? credentialProfilePath)
    {
        var profilePath = ResolveProfilePath(credentialProfilePath);
        var explicitProfilePath = !string.IsNullOrWhiteSpace(credentialProfilePath);

        if (!File.Exists(profilePath))
        {
            if (explicitProfilePath)
            {
                return BitflyerCredentialResolution.Failure($"Credential profile does not exist: {profilePath}");
            }

            return BitflyerCredentialResolution.None();
        }

        if (!CredentialProfileLoader.TryLoad(profilePath, out var profile, out var errorMessage))
        {
            return BitflyerCredentialResolution.Failure($"Invalid credential profile: {errorMessage}");
        }

        try
        {
            return BitflyerCredentialResolution.Success(
                CredentialProfileProviderFactory.Create(
                    profile!,
                    ExchangeVenue.Bitflyer,
                    profilePath,
                    new AgeCliCredentialFileDecryptor()));
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return BitflyerCredentialResolution.Failure(ex.Message);
        }
    }

    public static string BuildMissingCredentialMessage()
    {
        return CredentialProfileDefaults.GetMissingCredentialMessage(ExchangeVenue.Bitflyer);
    }

    private static string ResolveProfilePath(string? credentialProfilePath)
    {
        return string.IsNullOrWhiteSpace(credentialProfilePath)
            ? CredentialProfileDefaults.ResolveDefaultProfilePath()
            : credentialProfilePath;
    }
}

public sealed class BitflyerCredentialResolution
{
    private BitflyerCredentialResolution(IApiCredentialProvider? credentials, string? errorMessage)
    {
        Credentials = credentials;
        ErrorMessage = errorMessage;
    }

    public IApiCredentialProvider? Credentials { get; }

    public string? ErrorMessage { get; }

    public bool HasFailure => !string.IsNullOrWhiteSpace(ErrorMessage);

    public static BitflyerCredentialResolution Success(IApiCredentialProvider credentials)
    {
        return new BitflyerCredentialResolution(credentials, null);
    }

    public static BitflyerCredentialResolution None()
    {
        return new BitflyerCredentialResolution(null, null);
    }

    public static BitflyerCredentialResolution Failure(string message)
    {
        return new BitflyerCredentialResolution(null, message);
    }
}
