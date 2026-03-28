using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Commands;
using ExchangeApi.Adapters.Cli.Formatting;
using ExchangeApi.Adapters.Cli.Help;
using ExchangeApi.Adapters.Cli.Safety;
using ExchangeApi.Adapters.Cli.Shell;
using ExchangeApi.Adapters.Cli.Wizard;

namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class CliApplication
{
    private readonly IReadOnlyList<CommandDescriptor> _commands;
    private readonly IReadOnlyDictionary<string, CliOptionSpec> _allKnownOptions;
    private readonly IConsole _console;
    private readonly IEnvironment _environment;

    public CliApplication(
        IReadOnlyList<CommandDescriptor>? commands = null,
        IConsole? console = null,
        IEnvironment? environment = null)
    {
        _commands = commands ?? CommandCatalog.All;
        _allKnownOptions = CliOptionCatalog.BuildAllKnown(_commands);
        _console = console ?? new SystemConsole();
        _environment = environment ?? new ProcessEnvironment();
    }

    public static Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
    {
        return new CliApplication().RunInternalAsync(args, cancellationToken);
    }

    public Task<int> RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken = default)
    {
        return RunInternalAsync(args.ToArray(), cancellationToken);
    }

    private async Task<int> RunInternalAsync(string[] args, CancellationToken cancellationToken)
    {
        try
        {
            var shellResult = await ShellRunner.TryRunAsync(
                args,
                _commands,
                _console,
                RunCanonicalAsync,
                cancellationToken);
            if (shellResult.Handled)
            {
                return shellResult.ExitCode;
            }

            var wizardResult = await WizardRunner.TryRunAsync(args, _commands, _console, cancellationToken);
            if (wizardResult.Handled)
            {
                return wizardResult.ExitCode;
            }

            return await RunCanonicalAsync(args, cancellationToken);
        }
        catch (Exception ex)
        {
            StderrWriter.WriteFailure(
                _console,
                ExecutionOutcome.Unexpected("unexpected internal error", ex.Message),
                verbose: true);
            return CliExitCode.UnexpectedInternalError;
        }
    }

    private async Task<int> RunCanonicalAsync(IReadOnlyList<string> args, CancellationToken cancellationToken)
    {
        var parseResult = InvocationParser.Parse(args.ToArray(), _allKnownOptions);
        if (!parseResult.IsSuccess)
        {
            StderrWriter.WriteFailure(
                _console,
                ExecutionOutcome.InputError(parseResult.ErrorSummary ?? "invalid argument", parseResult.ErrorDetail),
                verbose: false);
            return CliExitCode.ArgumentConfigOrSafetyError;
        }

        if (parseResult.ShowHelp)
        {
            var helpOutcome = HelpRenderer.Render(_console, _commands, parseResult.PathTokens);
            if (helpOutcome.ExitCode != CliExitCode.Success)
            {
                StderrWriter.WriteFailure(_console, helpOutcome, verbose: false);
                return helpOutcome.ExitCode;
            }

            return CliExitCode.Success;
        }

        var path = new CommandPath(
            parseResult.PathTokens[0],
            parseResult.PathTokens[1],
            parseResult.PathTokens[2],
            parseResult.PathTokens[3]);

        var descriptor = _commands.FirstOrDefault(x => x.Path == path);
        if (descriptor is null)
        {
            StderrWriter.WriteFailure(
                _console,
                ExecutionOutcome.InputError("invalid argument", $"unknown command: {path.Identity}"),
                verbose: false);
            return CliExitCode.ArgumentConfigOrSafetyError;
        }

        var options = parseResult.Options;
        var optionValidationFailure = CliOptionCatalog.ValidateForCommand(descriptor, options);
        if (optionValidationFailure is not null)
        {
            StderrWriter.WriteFailure(_console, optionValidationFailure, verbose: options.HasFlag("verbose"));
            return optionValidationFailure.ExitCode;
        }

        var templateOutcome = TryHandleTemplate(descriptor, options);
        if (templateOutcome.Handled)
        {
            if (templateOutcome.Outcome!.ExitCode != CliExitCode.Success)
            {
                StderrWriter.WriteFailure(_console, templateOutcome.Outcome, verbose: options.HasFlag("verbose"));
                return templateOutcome.Outcome.ExitCode;
            }

            JsonOutputWriter.Write(_console, templateOutcome.TemplateDocument, pretty: options.HasFlag("pretty"));
            return CliExitCode.Success;
        }

        var requestBinding = await descriptor.BindRequestAsync(options, _console, cancellationToken);
        if (!requestBinding.IsSuccess)
        {
            StderrWriter.WriteFailure(
                _console,
                ExecutionOutcome.InputError(requestBinding.ErrorSummary ?? "invalid argument", requestBinding.ErrorDetail),
                verbose: options.HasFlag("verbose"));
            return CliExitCode.ArgumentConfigOrSafetyError;
        }

        if (descriptor.IsWrite && !options.HasFlag("yes"))
        {
            if (!ConfirmationPrompt.IsInteractive(_console))
            {
                StderrWriter.WriteFailure(
                    _console,
                    ExecutionOutcome.InputError("safety error", "--yes is required for non-interactive write execution"),
                    verbose: options.HasFlag("verbose"));
                return CliExitCode.ArgumentConfigOrSafetyError;
            }

            var confirmed = await ConfirmationPrompt.ConfirmAsync(
                _console,
                descriptor,
                requestBinding.Request!,
                cancellationToken);
            if (!confirmed)
            {
                StderrWriter.WriteFailure(
                    _console,
                    ExecutionOutcome.InputError($"{descriptor.Path.Identity}: confirmation declined"),
                    verbose: options.HasFlag("verbose"));
                return CliExitCode.ArgumentConfigOrSafetyError;
            }
        }

        var outcome = await descriptor.ExecuteAsync(options, requestBinding.Request!, _environment, cancellationToken);
        if (outcome.ExitCode != CliExitCode.Success)
        {
            StderrWriter.WriteFailure(_console, outcome, verbose: options.HasFlag("verbose"));
            return outcome.ExitCode;
        }

        JsonOutputWriter.Write(_console, outcome.Response, pretty: options.HasFlag("pretty"));
        if (options.HasFlag("summary"))
        {
            StderrWriter.WriteSuccessSummary(_console, outcome.Summary);
        }

        return CliExitCode.Success;
    }

    private static TemplateOutcome TryHandleTemplate(CommandDescriptor descriptor, InvocationOptions options)
    {
        var requestTemplate = options.HasFlag("request-template");
        var queryTemplate = options.HasFlag("query-template");
        var bodyTemplate = options.HasFlag("body-template");
        var count = (requestTemplate ? 1 : 0) + (queryTemplate ? 1 : 0) + (bodyTemplate ? 1 : 0);

        if (count == 0)
        {
            return TemplateOutcome.NotHandled();
        }

        if (count > 1)
        {
            return TemplateOutcome.HandledWith(ExecutionOutcome.InputError("invalid argument", "template options cannot be combined"));
        }

        var requestedTemplateOption = requestTemplate
            ? "request-template"
            : queryTemplate
                ? "query-template"
                : "body-template";
        var supportedTemplateOption = GetTemplateOptionName(descriptor.InputMode);
        if (!string.Equals(requestedTemplateOption, supportedTemplateOption, StringComparison.Ordinal))
        {
            return TemplateOutcome.HandledWith(
                ExecutionOutcome.InputError(
                    "invalid argument",
                    $"{descriptor.Path.Identity} only supports --{supportedTemplateOption}"));
        }

        var mixedInput = HasMixedInput(descriptor, options);
        if (mixedInput)
        {
            return TemplateOutcome.HandledWith(ExecutionOutcome.InputError("invalid argument", "template option cannot be combined with canonical input"));
        }

        return TemplateOutcome.HandledWithSuccess(descriptor.TemplateJson);
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

    private static bool HasMixedInput(CommandDescriptor descriptor, InvocationOptions options)
    {
        return descriptor.InputMode switch
        {
            CommandInputMode.NativeRequest => options.Contains("request-json")
                || options.Contains("request-file")
                || descriptor.CommandOptions.Any(option => options.Contains(option.Name)),
            CommandInputMode.ProtocolQuery => options.Contains("query-json")
                || options.Contains("query-file")
                || descriptor.CommandOptions.Any(option => options.Contains(option.Name)),
            CommandInputMode.ProtocolBody => options.Contains("body-json")
                || options.Contains("body-file")
                || descriptor.CommandOptions.Any(option => options.Contains(option.Name)),
            _ => false,
        };
    }

    private sealed class TemplateOutcome
    {
        public required bool Handled { get; init; }
        public ExecutionOutcome? Outcome { get; init; }
        public object? TemplateDocument { get; init; }

        public static TemplateOutcome NotHandled()
        {
            return new TemplateOutcome { Handled = false };
        }

        public static TemplateOutcome HandledWith(ExecutionOutcome outcome)
        {
            return new TemplateOutcome
            {
                Handled = true,
                Outcome = outcome,
            };
        }

        public static TemplateOutcome HandledWithSuccess(string templateJson)
        {
            return new TemplateOutcome
            {
                Handled = true,
                Outcome = ExecutionOutcome.Success("template rendered", null),
                TemplateDocument = System.Text.Json.JsonSerializer.Deserialize<object>(templateJson),
            };
        }
    }
}
