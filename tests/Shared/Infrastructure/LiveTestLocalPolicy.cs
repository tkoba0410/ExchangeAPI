namespace ExchangeApi.Tests.LiveTests.Infrastructure;

internal static class LiveTestLocalPolicy
{
    public const string RunLiveTestsEnvironmentVariableName = "EXCHANGEAPI_RUN_LIVE_TESTS";
    private const string LiveOptInMarkerFileName = "live-enabled";

    public static bool HasLiveOptIn()
    {
        return IsTruthy(Environment.GetEnvironmentVariable(RunLiveTestsEnvironmentVariableName))
            || HasMarker(LiveOptInMarkerFileName);
    }

    public static string LiveOptInMessage(string target)
    {
        return $"Set {RunLiveTestsEnvironmentVariableName}=1 or create local/{LiveOptInMarkerFileName} to run {target}.";
    }

    public static bool HasMarker(string markerFileName)
    {
        return File.Exists(LocalPath(markerFileName));
    }

    public static string LocalPath(params string[] relativeSegments)
    {
        return Path.Combine([RepoRoot(), "local", .. relativeSegments]);
    }

    private static bool IsTruthy(string? value)
    {
        return string.Equals(value, "1", StringComparison.Ordinal)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
    }

    private static string RepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "ExchangeApi.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("Repository root was not found.");
    }
}
