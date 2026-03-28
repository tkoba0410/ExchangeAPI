using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Help;

public static class HelpRenderer
{
    public static ExecutionOutcome Render(
        IConsole console,
        IReadOnlyList<CommandDescriptor> commands,
        IReadOnlyList<string> pathTokens)
    {
        if (pathTokens.Count == 0)
        {
            RenderRoot(console, commands);
            return ExecutionOutcome.Success("help rendered", null);
        }

        if (pathTokens.Count == 1)
        {
            return RenderVenue(console, commands, pathTokens[0]);
        }

        if (pathTokens.Count == 2)
        {
            return RenderSurface(console, commands, pathTokens[0], pathTokens[1]);
        }

        if (pathTokens.Count == 3)
        {
            return RenderScope(console, commands, pathTokens[0], pathTokens[1], pathTokens[2]);
        }

        return RenderCommand(console, commands, new CommandPath(pathTokens[0], pathTokens[1], pathTokens[2], pathTokens[3]));
    }

    private static void RenderRoot(IConsole console, IReadOnlyList<CommandDescriptor> commands)
    {
        console.WriteOutLine("Usage:");
        console.WriteOutLine("  exchangeapi <venue> <surface> <scope> <command> [options]");
        console.WriteOutLine("  exchangeapi wizard <venue> <surface> <scope> <command>");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Examples:");
        console.WriteOutLine("  exchangeapi bitflyer native public get-ticker --product-code BTC_JPY");
        console.WriteOutLine("  exchangeapi wizard bitflyer native public get-ticker");
        console.WriteOutLine("  exchangeapi shell");
        console.WriteOutLine("  exchangeapi bitflyer native private cancel-all-child-orders --product-code BTC_JPY --yes");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Available venues:");
        foreach (var venue in commands.Select(static x => x.Path.Venue).Distinct(StringComparer.Ordinal).OrderBy(static x => x, StringComparer.Ordinal))
        {
            console.WriteOutLine($"  {venue}");
        }
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Other interfaces:");
        console.WriteOutLine("  wizard");
        console.WriteOutLine("  shell");
    }

    private static ExecutionOutcome RenderVenue(IConsole console, IReadOnlyList<CommandDescriptor> commands, string venue)
    {
        var matches = commands.Where(x => x.Path.Venue == venue).ToArray();
        if (matches.Length == 0)
        {
            return ExecutionOutcome.InputError("invalid argument", $"unknown venue: {venue}");
        }

        console.WriteOutLine("Usage:");
        console.WriteOutLine($"  exchangeapi {venue} <surface> <scope> <command> [options]");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Available surfaces:");
        foreach (var surface in matches.Select(static x => x.Path.Surface).Distinct(StringComparer.Ordinal).OrderBy(static x => x, StringComparer.Ordinal))
        {
            console.WriteOutLine($"  {surface}");
        }

        console.WriteOutLine(string.Empty);
        console.WriteOutLine($"Example:");
        console.WriteOutLine($"  exchangeapi {venue} {matches[0].Path.Surface} --help");
        return ExecutionOutcome.Success("help rendered", null);
    }

    private static ExecutionOutcome RenderSurface(IConsole console, IReadOnlyList<CommandDescriptor> commands, string venue, string surface)
    {
        var matches = commands.Where(x => x.Path.Venue == venue && x.Path.Surface == surface).ToArray();
        if (matches.Length == 0)
        {
            return ExecutionOutcome.InputError("invalid argument", $"unknown surface: {venue} {surface}");
        }

        console.WriteOutLine("Usage:");
        console.WriteOutLine($"  exchangeapi {venue} {surface} <scope> <command> [options]");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Available scopes:");
        foreach (var scope in matches.Select(static x => x.Path.Scope).Distinct(StringComparer.Ordinal).OrderBy(static x => x, StringComparer.Ordinal))
        {
            console.WriteOutLine($"  {scope}");
        }

        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Example:");
        console.WriteOutLine($"  exchangeapi {venue} {surface} {matches[0].Path.Scope} --help");
        return ExecutionOutcome.Success("help rendered", null);
    }

    private static ExecutionOutcome RenderScope(IConsole console, IReadOnlyList<CommandDescriptor> commands, string venue, string surface, string scope)
    {
        var matches = commands.Where(x => x.Path.Venue == venue && x.Path.Surface == surface && x.Path.Scope == scope).ToArray();
        if (matches.Length == 0)
        {
            return ExecutionOutcome.InputError("invalid argument", $"unknown scope: {venue} {surface} {scope}");
        }

        console.WriteOutLine("Usage:");
        console.WriteOutLine($"  exchangeapi {venue} {surface} {scope} <command> [options]");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Available commands:");
        foreach (var command in matches.OrderBy(static x => x.Path.Command, StringComparer.Ordinal))
        {
            console.WriteOutLine($"  {command.Path.Command}");
        }

        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Example:");
        console.WriteOutLine($"  exchangeapi {venue} {surface} {scope} {matches[0].Path.Command} --help");
        return ExecutionOutcome.Success("help rendered", null);
    }

    private static ExecutionOutcome RenderCommand(IConsole console, IReadOnlyList<CommandDescriptor> commands, CommandPath path)
    {
        var descriptor = commands.FirstOrDefault(x => x.Path == path);
        if (descriptor is null)
        {
            return ExecutionOutcome.InputError("invalid argument", $"unknown command: {path.Identity}");
        }

        console.WriteOutLine("Usage:");
        console.WriteOutLine($"  exchangeapi {path.Identity} [options]");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine(descriptor.Summary);
        console.WriteOutLine(string.Empty);
        console.WriteOutLine($"Authentication: {descriptor.AuthenticationRequirement}");
        console.WriteOutLine($"Write safety: {(descriptor.IsWrite ? "--yes required or interactive confirmation" : "read-only")}");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Canonical input example:");
        console.WriteOutLine($"  {descriptor.CanonicalJsonExample}");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Convenience flags:");
        foreach (var flag in descriptor.CommandOptions.Select(static x => x.DisplayText))
        {
            console.WriteOutLine($"  {flag}");
        }

        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Template:");
        console.WriteOutLine($"  --{GetTemplateOptionName(descriptor.InputMode)}");
        console.WriteOutLine(string.Empty);

        if (string.Equals(path.Surface, "protocol", StringComparison.Ordinal))
        {
            console.WriteOutLine("Protocol semantics:");
            console.WriteOutLine("  stdout schema: Request / Response / Meta");
            console.WriteOutLine("  Response.BodyText: raw string");
            console.WriteOutLine("  inspect HTTP status via Response.StatusCode");
            console.WriteOutLine("  non-success HTTP status alone does not cause exit code 3");
            console.WriteOutLine(string.Empty);
        }

        console.WriteOutLine("Examples:");
        foreach (var usageExample in descriptor.UsageExamples)
        {
            console.WriteOutLine($"  {usageExample}");
        }

        return ExecutionOutcome.Success("help rendered", null);
    }

    private static string GetTemplateOptionName(CommandInputMode inputMode)
    {
        return inputMode switch
        {
            CommandInputMode.NativeRequest => "request-template",
            CommandInputMode.ProtocolQuery => "query-template",
            CommandInputMode.ProtocolBody => "body-template",
            _ => "request-template",
        };
    }
}
