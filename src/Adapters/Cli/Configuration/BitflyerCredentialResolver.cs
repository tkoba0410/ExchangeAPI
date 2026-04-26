using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Optional.Credentials;
using ExchangeApi.Optional.Credentials.AgeFile;
using ExchangeApi.Optional.Credentials.Profiles;
using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Adapters.Cli.Configuration;

public static class BitflyerCredentialResolver
{
    public const int CanonicalVersion = 1;
    public const string CanonicalVenue = "bitflyer";
    public const string CredentialProfileOptionName = "credential-profile";
    public static readonly string DefaultCredentialProfilePath = CredentialProfileDefaults.DefaultProfilePath;
    public static readonly string AuthenticationRequirementText =
        $"--{CredentialProfileOptionName} / {CredentialProfileDefaults.DefaultProfilePath}";

    public static BitflyerCredentialResolution Resolve(IEnvironment environment)
    {
        return Resolve(environment, null, ProcessAgeCredentialDecryptor.Instance);
    }

    public static BitflyerCredentialResolution Resolve(IEnvironment environment, IAgeCredentialDecryptor decryptor)
    {
        return Resolve(environment, null, decryptor);
    }

    public static BitflyerCredentialResolution Resolve(
        IEnvironment environment,
        string? credentialProfilePath,
        IAgeCredentialDecryptor decryptor)
    {
        var explicitProfilePath = !string.IsNullOrWhiteSpace(credentialProfilePath);
        if (!explicitProfilePath && !environment.AllowDefaultCredentialProfileDiscovery)
        {
            return BitflyerCredentialResolution.None();
        }

        var profilePath = string.IsNullOrWhiteSpace(credentialProfilePath)
            ? CredentialProfileDefaults.ResolveDefaultProfilePath()
            : credentialProfilePath;

        if (!File.Exists(profilePath))
        {
            if (explicitProfilePath)
            {
                return BitflyerCredentialResolution.Failure($"Credential profile does not exist: {profilePath}");
            }

            return BitflyerCredentialResolution.None();
        }

        if (!decryptor.IsAvailable())
        {
            return BitflyerCredentialResolution.Failure("The 'age' executable was not found on PATH.");
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
                    new CliAgeCredentialFileDecryptor(decryptor)));
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

    private sealed class CliAgeCredentialFileDecryptor : IAgeCredentialFileDecryptor
    {
        private readonly IAgeCredentialDecryptor _decryptor;

        public CliAgeCredentialFileDecryptor(IAgeCredentialDecryptor decryptor)
        {
            _decryptor = decryptor;
        }

        public string Decrypt(string identityFilePath, string credentialsFilePath)
        {
            return _decryptor.Decrypt(identityFilePath, credentialsFilePath);
        }
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
