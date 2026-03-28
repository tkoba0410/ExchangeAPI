using System.Text.Json;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Adapters.Cli.Formatting;

namespace ExchangeApi.Adapters.Cli.Wizard;

public static class WizardRunner
{
    public static async Task<SpecialCommandResult> TryRunAsync(
        IReadOnlyList<string> args,
        IReadOnlyList<CommandDescriptor> commands,
        IConsole console,
        CancellationToken cancellationToken)
    {
        if (args.Count == 0 || !string.Equals(args[0], "wizard", StringComparison.Ordinal))
        {
            return SpecialCommandResult.NotHandled();
        }

        var wizardArgs = args.Skip(1).ToArray();
        if (wizardArgs.Length == 0
            || wizardArgs[0] is "help" or "--help" or "-h")
        {
            RenderRootHelp(console, commands);
            return SpecialCommandResult.FromExitCode(CliExitCode.Success);
        }

        if (wizardArgs.Length != 4)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", "wizard requires <venue> <surface> <scope> <command>"),
                verbose: false);
            return SpecialCommandResult.FromExitCode(CliExitCode.ArgumentConfigOrSafetyError);
        }

        var path = new CommandPath(wizardArgs[0], wizardArgs[1], wizardArgs[2], wizardArgs[3]);
        var descriptor = commands.FirstOrDefault(x => x.Path == path);
        if (descriptor is null)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", $"unknown command: {path.Identity}"),
                verbose: false);
            return SpecialCommandResult.FromExitCode(CliExitCode.ArgumentConfigOrSafetyError);
        }

        if (descriptor.Wizard is null)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError("invalid argument", $"wizard is not available for {path.Identity}"),
                verbose: false);
            return SpecialCommandResult.FromExitCode(CliExitCode.ArgumentConfigOrSafetyError);
        }

        var invocationOptions = await PromptAsync(console, descriptor, descriptor.Wizard, cancellationToken);
        var requestBinding = await descriptor.BindRequestAsync(invocationOptions, console, cancellationToken);
        if (!requestBinding.IsSuccess)
        {
            StderrWriter.WriteFailure(
                console,
                ExecutionOutcome.InputError(requestBinding.ErrorSummary ?? "invalid argument", requestBinding.ErrorDetail),
                verbose: false);
            return SpecialCommandResult.FromExitCode(CliExitCode.ArgumentConfigOrSafetyError);
        }

        var requestJson = JsonSerializer.Serialize(requestBinding.Request, requestBinding.Request!.GetType());
        var canonicalOptionName = descriptor.Wizard.CanonicalInputKind switch
        {
            WizardCanonicalInputKind.RequestJson => "--request-json",
            WizardCanonicalInputKind.QueryJson => "--query-json",
            _ => "--request-json",
        };

        var commandLine = $"exchangeapi {path.Identity} {canonicalOptionName} {QuoteForShell(requestJson)}";
        console.WriteOutLine(commandLine);

        console.WriteErrorLine($"wizard generated canonical command for {path.Identity}");
        if (!string.IsNullOrWhiteSpace(descriptor.Wizard.CompletionNote))
        {
            console.WriteErrorLine(descriptor.Wizard.CompletionNote);
        }

        return SpecialCommandResult.FromExitCode(CliExitCode.Success);
    }

    private static async Task<InvocationOptions> PromptAsync(
        IConsole console,
        CommandDescriptor descriptor,
        WizardDefinition wizard,
        CancellationToken cancellationToken)
    {
        console.WriteErrorLine($"Wizard: {descriptor.Path.Identity}");
        console.WriteErrorLine(wizard.Summary);

        var options = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var field in wizard.Fields)
        {
            while (true)
            {
                var suffix = field.Required ? "required" : "optional";
                var hint = string.IsNullOrWhiteSpace(field.Hint) ? string.Empty : $" ({field.Hint})";
                console.WriteErrorLine($"{field.Prompt} [{suffix}]{hint}");

                var value = await console.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (field.Required)
                    {
                        console.WriteErrorLine($"{field.OptionName}: value required");
                        continue;
                    }

                    break;
                }

                options[field.OptionName] = value.Trim();
                break;
            }
        }

        return new InvocationOptions(options);
    }

    private static void RenderRootHelp(IConsole console, IReadOnlyList<CommandDescriptor> commands)
    {
        console.WriteOutLine("Usage:");
        console.WriteOutLine("  exchangeapi wizard <venue> <surface> <scope> <command>");
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Wizard-supported commands:");
        foreach (var descriptor in commands.Where(static x => x.Wizard is not null).OrderBy(static x => x.Path.Identity, StringComparer.Ordinal))
        {
            console.WriteOutLine($"  {descriptor.Path.Identity}");
        }
        console.WriteOutLine(string.Empty);
        console.WriteOutLine("Wizard prints an equivalent canonical command.");
    }

    private static string QuoteForShell(string text)
    {
        return "'" + text.Replace("'", "'\"'\"'") + "'";
    }
}
