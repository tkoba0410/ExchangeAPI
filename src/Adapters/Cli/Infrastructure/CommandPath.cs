namespace ExchangeApi.Adapters.Cli.Infrastructure;

public readonly record struct CommandPath(
    string Venue,
    string Surface,
    string Scope,
    string Command)
{
    public string Identity => $"{Venue} {Surface} {Scope} {Command}";
}
