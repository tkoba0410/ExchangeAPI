namespace ExchangeApi.Optional.Credentials.Profiles;

public static class CredentialProfileDefaults
{
    public static readonly string DefaultProfilePath = Path.Combine("local", "credentials", "credential-profile.json");

    public static readonly string DefaultCurrentDirectory = Path.Combine("local", "credentials", "current");

    public const string DefaultAgeIdentityFileName = "age-identity.txt";

    public static string GetVenueId(ExchangeVenue venue)
    {
        return venue switch
        {
            ExchangeVenue.Bitflyer => "bitflyer",
            ExchangeVenue.Binance => "binance",
            _ => throw new ArgumentOutOfRangeException(nameof(venue), venue, "Unsupported venue."),
        };
    }

    public static string GetDefaultCredentialsFileName(ExchangeVenue venue)
    {
        return $"{GetVenueId(venue)}.age";
    }

    public static string ResolveDefaultProfilePath()
    {
        if (File.Exists(DefaultProfilePath))
        {
            return DefaultProfilePath;
        }

        foreach (var root in EnumerateSearchRoots())
        {
            var candidate = FindInParents(root, DefaultProfilePath);
            if (candidate is not null)
            {
                return candidate;
            }
        }

        return DefaultProfilePath;
    }

    public static string GetMissingCredentialMessage(ExchangeVenue venue, string? profilePath = null)
    {
        var path = string.IsNullOrWhiteSpace(profilePath)
            ? DefaultProfilePath
            : profilePath;

        return $"Configure --credential-profile <path> or {path}. Expected symlinks under {DefaultCurrentDirectory}/: {DefaultAgeIdentityFileName} and {GetDefaultCredentialsFileName(venue)}.";
    }

    private static IEnumerable<string> EnumerateSearchRoots()
    {
        yield return Directory.GetCurrentDirectory();
        yield return AppContext.BaseDirectory;
    }

    private static string? FindInParents(string startDirectory, string relativePath)
    {
        var directory = new DirectoryInfo(Path.GetFullPath(startDirectory));
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
