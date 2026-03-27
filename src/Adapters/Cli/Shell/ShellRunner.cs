using ExchangeApi.Adapters.Cli.Formatting;
using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Shell;

public static class ShellRunner
{
    private static readonly string[] BuiltInCommands =
    [
        "help",
        "show",
        "use venue <value>",
        "use surface <value>",
        "use scope <value>",
        "run <command> [options]",
        "run <venue> <surface> <scope> <command> [options]",
        "exit",
        "quit",
    ];

    public static async Task<SpecialCommandResult> TryRunAsync(
        IReadOnlyList<string> args,
        IReadOnlyList<CommandDescriptor> commands,
        IConsole console,
        Func<IReadOnlyList<string>, CancellationToken, Task<int>> executeCanonicalAsync,
        CancellationToken cancellationToken)
    {
        if (args.Count == 0 || !string.Equals(args[0], "shell", StringComparison.Ordinal))
        {
            return SpecialCommandResult.NotHandled();
        }

        if (args.Count > 1)
        {
            if (args.Count == 2 && args[1] is "help" or "--help" or "-h")
            {
                RenderHelp(console, commands);
                return SpecialCommandResult.FromExitCode(CliExitCode.Success);
            }

            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", "shell does not accept positional arguments"),
                verbose: false);
            return SpecialCommandResult.FromExitCode(CliExitCode.ArgumentConfigOrSafetyError);
        }

        console.WriteErrorLine("Shell helper started.");
        console.WriteErrorLine("Type 'help' for commands or 'exit' to leave.");

        var state = new ShellSessionState();
        while (true)
        {
            console.WriteError("exchangeapi> ");
            var line = await console.ReadLineAsync(cancellationToken);
            if (line is null)
            {
                console.WriteErrorLine(string.Empty);
                return SpecialCommandResult.FromExitCode(CliExitCode.Success);
            }

            if (!ShellLineTokenizer.TryTokenize(line, out var tokens, out var errorDetail))
            {
                StderrWriter.WriteFailure(
                    console,
                    ExecutionOutcome.InputError("invalid argument", errorDetail),
                    verbose: false);
                continue;
            }

            if (tokens.Count == 0)
            {
                continue;
            }

            if (IsExit(tokens[0]))
            {
                console.WriteErrorLine("Shell helper finished.");
                return SpecialCommandResult.FromExitCode(CliExitCode.Success);
            }

            await HandleCommandAsync(tokens, state, commands, console, executeCanonicalAsync, cancellationToken);
        }
    }

    private static async Task HandleCommandAsync(
        IReadOnlyList<string> tokens,
        ShellSessionState state,
        IReadOnlyList<CommandDescriptor> commands,
        IConsole console,
        Func<IReadOnlyList<string>, CancellationToken, Task<int>> executeCanonicalAsync,
        CancellationToken cancellationToken)
    {
        switch (tokens[0])
        {
            case "help":
                RenderHelp(console, commands);
                return;

            case "show":
                console.WriteErrorLine(state.Describe());
                return;

            case "use":
                HandleUse(tokens, state, commands, console);
                return;

            case "run":
                await HandleRunAsync(tokens.Skip(1).ToArray(), state, commands, console, executeCanonicalAsync, cancellationToken);
                return;

            default:
                StderrWriter.WriteFailure(
                    console,
                    ExecutionOutcome.InputError("invalid argument", $"unknown shell command: {tokens[0]}"),
                    verbose: false);
                return;
        }
    }

    private static void HandleUse(
        IReadOnlyList<string> tokens,
        ShellSessionState state,
        IReadOnlyList<CommandDescriptor> commands,
        IConsole console)
    {
        if (tokens.Count != 3)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", "use requires <venue|surface|scope> <value>"),
                verbose: false);
            return;
        }

        var kind = tokens[1];
        var value = tokens[2];
        switch (kind)
        {
            case "venue":
                if (!commands.Any(x => x.Path.Venue == value))
                {
                    StderrWriter.WriteFailure(
                        console,
                        ExecutionOutcome.InputError("invalid argument", $"unknown venue: {value}"),
                        verbose: false);
                    return;
                }

                state.SetVenue(value);
                console.WriteErrorLine($"venue={value}");
                return;

            case "surface":
                if (!commands.Any(x => x.Path.Surface == value))
                {
                    StderrWriter.WriteFailure(
                        console,
                        ExecutionOutcome.InputError("invalid argument", $"unknown surface: {value}"),
                        verbose: false);
                    return;
                }

                state.SetSurface(value);
                console.WriteErrorLine($"surface={value}");
                return;

            case "scope":
                if (!commands.Any(x => x.Path.Scope == value))
                {
                    StderrWriter.WriteFailure(
                        console,
                        ExecutionOutcome.InputError("invalid argument", $"unknown scope: {value}"),
                        verbose: false);
                    return;
                }

                state.SetScope(value);
                console.WriteErrorLine($"scope={value}");
                return;

            default:
                StderrWriter.WriteFailure(
                    console,
                    ExecutionOutcome.InputError("invalid argument", $"unknown use target: {kind}"),
                    verbose: false);
                return;
        }
    }

    private static async Task HandleRunAsync(
        IReadOnlyList<string> tokens,
        ShellSessionState state,
        IReadOnlyList<CommandDescriptor> commands,
        IConsole console,
        Func<IReadOnlyList<string>, CancellationToken, Task<int>> executeCanonicalAsync,
        CancellationToken cancellationToken)
    {
        if (tokens.Count == 0)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", "run requires a command path or command name"),
                verbose: false);
            return;
        }

        CommandPath path;
        string[] optionTokens;
        if (HasExplicitPath(tokens))
        {
            path = new CommandPath(tokens[0], tokens[1], tokens[2], tokens[3]);
            optionTokens = tokens.Skip(4).ToArray();
        }
        else
        {
            if (state.Venue is null || state.Surface is null || state.Scope is null)
            {
                StderrWriter.WriteFailure(
                    console,
                    ExecutionOutcome.InputError("invalid argument", "run <command> requires venue, surface, and scope defaults"),
                    verbose: false);
                return;
            }

            path = new CommandPath(state.Venue, state.Surface, state.Scope, tokens[0]);
            optionTokens = tokens.Skip(1).ToArray();
        }

        var descriptor = commands.FirstOrDefault(x => x.Path == path);
        if (descriptor is null)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", $"unknown command: {path.Identity}"),
                verbose: false);
            return;
        }

        var canonicalArgs = new List<string>(capacity: 4 + optionTokens.Length)
        {
            path.Venue,
            path.Surface,
            path.Scope,
            path.Command,
        };
        canonicalArgs.AddRange(optionTokens);

        var exitCode = await executeCanonicalAsync(canonicalArgs, cancellationToken);
        state.SetLastExitCode(exitCode);

        if (exitCode == CliExitCode.Success)
        {
            console.WriteErrorLine($"shell executed: {path.Identity}");
            return;
        }

        console.WriteErrorLine($"shell failed: {path.Identity} exit={exitCode}");
    }

    private static bool HasExplicitPath(IReadOnlyList<string> tokens)
    {
        return tokens.Count >= 4
            && !tokens[0].StartsWith("--", StringComparison.Ordinal)
            && !tokens[1].StartsWith("--", StringComparison.Ordinal)
            && !tokens[2].StartsWith("--", StringComparison.Ordinal)
            && !tokens[3].StartsWith("--", StringComparison.Ordinal);
    }

    private static bool IsExit(string token)
    {
        return token is "exit" or "quit";
    }

    private static void RenderHelp(IConsole console, IReadOnlyList<CommandDescriptor> commands)
    {
        console.WriteErrorLine("Usage:");
        console.WriteErrorLine("  exchangeapi shell");
        console.WriteErrorLine(string.Empty);
        console.WriteErrorLine("Built-in commands:");
        foreach (var command in BuiltInCommands)
        {
            console.WriteErrorLine($"  {command}");
        }

        console.WriteErrorLine(string.Empty);
        console.WriteErrorLine("Available venues:");
        foreach (var venue in commands.Select(static x => x.Path.Venue).Distinct(StringComparer.Ordinal).OrderBy(static x => x, StringComparer.Ordinal))
        {
            console.WriteErrorLine($"  {venue}");
        }
    }
}
