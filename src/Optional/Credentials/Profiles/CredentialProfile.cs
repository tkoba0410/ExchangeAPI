namespace ExchangeApi.Optional.Credentials.Profiles;

public sealed class CredentialProfile
{
    public int Version { get; init; } = 1;

    public IReadOnlyDictionary<string, CredentialProfileEntry> Credentials { get; init; }
        = new Dictionary<string, CredentialProfileEntry>(StringComparer.OrdinalIgnoreCase);
}

public sealed class CredentialProfileEntry
{
    public string Provider { get; init; } = CredentialProfileProviderNames.AgeFile;

    public string? IdentityFilePath { get; init; }

    public string? CredentialsFilePath { get; init; }
}

public static class CredentialProfileProviderNames
{
    public const string AgeFile = "age-file";
}
