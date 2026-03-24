namespace ExchangeApi.Tests.LiveTests.Infrastructure;

internal static class LiveTestLocalPolicy
{
    public static bool HasMarker(string markerFileName)
    {
        return File.Exists(LocalPath(markerFileName));
    }

    public static string LocalPath(params string[] relativeSegments)
    {
        return Path.Combine([RepoRoot(), "local", .. relativeSegments]);
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
