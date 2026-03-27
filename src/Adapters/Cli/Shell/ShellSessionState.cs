namespace ExchangeApi.Adapters.Cli.Shell;

public sealed class ShellSessionState
{
    public string? Venue { get; private set; }
    public string? Surface { get; private set; }
    public string? Scope { get; private set; }
    public int? LastExitCode { get; private set; }

    public void SetVenue(string venue)
    {
        Venue = venue;
    }

    public void SetSurface(string surface)
    {
        Surface = surface;
    }

    public void SetScope(string scope)
    {
        Scope = scope;
    }

    public void SetLastExitCode(int exitCode)
    {
        LastExitCode = exitCode;
    }

    public string Describe()
    {
        return $"venue={Venue ?? "(unset)"} surface={Surface ?? "(unset)"} scope={Scope ?? "(unset)"} last-exit-code={(LastExitCode?.ToString() ?? "(unset)")}";
    }
}
