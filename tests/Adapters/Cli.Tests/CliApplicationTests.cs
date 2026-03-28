using ExchangeApi.Adapters.Cli.Commands;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Adapters.Cli.Tests;

public sealed class CliApplicationTests
{
    [Fact]
    public async Task NoArgs_PrintsRootHelp()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync([]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("exchangeapi <venue> <surface> <scope> <command>", console.StdOut);
        Assert.Contains("bitflyer", console.StdOut);
        Assert.Contains("binance", console.StdOut);
    }

    [Fact]
    public async Task GetTickerTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-ticker", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetMarketsTemplate_PrintsEmptyCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-markets", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetAddressesTemplate_PrintsEmptyCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-addresses", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetTickerProtocolTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "public", "get-ticker", "--query-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetBoardStateTemplate_PrintsPascalCaseCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-board-state", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"ProductCode":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetChatsTemplate_PrintsPascalCaseCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-chats", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"FromDate":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetExecutionsPublicTemplate_PrintsSnakeCaseCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-executions-public", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":null,"count":null,"before":null,"after":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetFundingRateWithoutProductCode_FailsInputValidation()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-funding-rate"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: ProductCode", console.StdErr);
    }

    [Fact]
    public async Task GetExecutionsPublicRejectsInvalidCountValue()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-executions-public", "--count", "abc"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: count", console.StdErr);
    }

    [Fact]
    public async Task RejectsCommandInapplicableOption()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "public", "get-board", "--count", "10"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid option", console.StdErr);
        Assert.Contains("unsupported option for bitflyer native public get-board: --count", console.StdErr);
    }

    [Fact]
    public async Task RejectsProtocolBodyOptionOnQueryCommand()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "public", "get-ticker", "--body-json", "{}"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid option", console.StdErr);
        Assert.Contains("unsupported option for bitflyer protocol public get-ticker: --body-json", console.StdErr);
    }

    [Fact]
    public async Task ProtocolCommandHelp_ExplainsEnvelopeAndStatusSemantics()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "public", "get-ticker", "--help"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("Protocol semantics:", console.StdOut);
        Assert.Contains("stdout schema: Request / Response / Meta", console.StdOut);
        Assert.Contains("Response.BodyText: raw string", console.StdOut);
        Assert.Contains("inspect HTTP status via Response.StatusCode", console.StdOut);
        Assert.Contains("non-success HTTP status alone does not cause exit code 3", console.StdOut);
    }

    [Fact]
    public async Task RejectsMixedCanonicalJsonAndConvenienceFlags()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            [
                "bitflyer", "native", "public", "get-ticker",
                "--request-json", """{"product_code":"BTC_JPY"}""",
                "--product-code", "FX_BTC_JPY",
            ]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("convenience flags cannot be used together", console.StdErr);
    }

    [Fact]
    public async Task RejectsNonInteractiveWriteWithoutYes()
    {
        var console = new FakeConsole
        {
            IsInputRedirected = true,
            IsErrorRedirected = true,
        };
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "cancel-all-child-orders", "--product-code", "BTC_JPY"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("safety error", console.StdErr);
        Assert.Contains("--yes is required", console.StdErr);
    }

    [Fact]
    public async Task PrivateBitflyerCommand_RequiresCredentials()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            [
                "bitflyer", "native", "private", "cancel-all-child-orders",
                "--product-code", "BTC_JPY",
                "--yes",
            ]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("missing credential", console.StdErr);
        Assert.Contains("BITFLYER_API_KEY", console.StdErr);
    }

    [Fact]
    public async Task InjectedCommand_CanReturnSuccessfulJson()
    {
        var console = new FakeConsole();
        var app = new CliApplication(
            commands:
            [
                new CommandDescriptor
                {
                    Path = new CommandPath("fake", "native", "public", "echo"),
                    EndpointId = "Echo",
                    Summary = "fake command",
                    AuthenticationRequirement = "none",
                    InputMode = CommandInputMode.NativeRequest,
                    CanonicalJsonExample = "exchangeapi fake native public echo",
                    TemplateJson = "{}",
                    CommandOptions = [],
                    UsageExamples = ["exchangeapi fake native public echo --summary"],
                    IsWrite = false,
                    BindRequestAsync = static (_, _, _) => Task.FromResult(RequestBindingResult.Success(new object())),
                    DescribeRequest = static _ => "none",
                    ExecuteAsync = static (_, _, _, _) => Task.FromResult(
                        ExecutionOutcome.Success(
                            "fake native public echo: success",
                            new EchoResponse { Message = "ok" })),
                },
            ],
            console: console,
            environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["fake", "native", "public", "echo", "--summary", "--pretty"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains(Environment.NewLine, console.StdOut);
        Assert.Contains("ok", console.StdOut);
        Assert.Contains("fake native public echo: success", console.StdErr);
    }

    [Fact]
    public async Task WizardRootHelp_PrintsSupportedCommands()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["wizard"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("exchangeapi wizard <venue> <surface> <scope> <command>", console.StdOut);
        Assert.Contains("bitflyer native public get-ticker", console.StdOut);
    }

    [Fact]
    public void CommandCatalog_CurrentSlice_IsExpected()
    {
        var identities = CommandCatalog.All
            .Select(static x => x.Path.Identity)
            .OrderBy(static x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "binance native public get-klines",
                "binance protocol public get-klines",
                "bitflyer native private cancel-all-child-orders",
                "bitflyer native private get-addresses",
                "bitflyer native private get-balance",
                "bitflyer native private get-bank-accounts",
                "bitflyer native private get-collateral",
                "bitflyer native private get-collateral-accounts",
                "bitflyer native private get-permissions",
                "bitflyer native public get-board",
                "bitflyer native public get-board-state",
                "bitflyer native public get-chats",
                "bitflyer native public get-corporate-leverage",
                "bitflyer native public get-executions-public",
                "bitflyer native public get-funding-rate",
                "bitflyer native public get-health",
                "bitflyer native public get-markets",
                "bitflyer native public get-ticker",
                "bitflyer protocol public get-executions-public",
                "bitflyer protocol public get-markets",
                "bitflyer protocol public get-ticker",
            ],
            identities);
    }

    [Fact]
    public void CommandCatalog_CurrentSlice_MatchesDocumentedFamilies()
    {
        var grouped = CommandCatalog.All
            .GroupBy(static x => (x.Path.Venue, x.Path.Surface, x.Path.Scope))
            .ToDictionary(
                static x => x.Key,
                static x => x.Select(static y => y.Path.Command).OrderBy(static y => y, StringComparer.Ordinal).ToArray());

        Assert.Equal(
            [
                ("binance", "native", "public"),
                ("binance", "protocol", "public"),
                ("bitflyer", "native", "private"),
                ("bitflyer", "native", "public"),
                ("bitflyer", "protocol", "public"),
            ],
            grouped.Keys.OrderBy(static x => x, Comparer<(string, string, string)>.Default).ToArray());

        Assert.Equal(
            [
                "get-klines",
            ],
            grouped[("binance", "native", "public")]);

        Assert.Equal(
            [
                "get-klines",
            ],
            grouped[("binance", "protocol", "public")]);

        Assert.Equal(
            [
                "cancel-all-child-orders",
                "get-addresses",
                "get-balance",
                "get-bank-accounts",
                "get-collateral",
                "get-collateral-accounts",
                "get-permissions",
            ],
            grouped[("bitflyer", "native", "private")]);

        Assert.Equal(
            [
                "get-board",
                "get-board-state",
                "get-chats",
                "get-corporate-leverage",
                "get-executions-public",
                "get-funding-rate",
                "get-health",
                "get-markets",
                "get-ticker",
            ],
            grouped[("bitflyer", "native", "public")]);

        Assert.Equal(
            [
                "get-executions-public",
                "get-markets",
                "get-ticker",
            ],
            grouped[("bitflyer", "protocol", "public")]);
    }

    [Fact]
    public void CommandCatalog_WizardCoverage_IsDocumentedSubset()
    {
        var wizardIdentities = CommandCatalog.All
            .Where(static x => x.Wizard is not null)
            .Select(static x => x.Path.Identity)
            .OrderBy(static x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            [
                "binance native public get-klines",
                "bitflyer native private cancel-all-child-orders",
                "bitflyer native public get-ticker",
            ],
            wizardIdentities);
    }

    [Fact]
    public void ProtocolExecutionOutcome_WrapsRequestResponseAndMeta()
    {
        var call = new Call<ProtocolRequest, ProtocolResponse>
        {
            Request = new ProtocolRequest
            {
                EndpointId = "GetTicker",
                Method = "GET",
                Path = "/v1/getticker",
                Query = new Dictionary<string, string> { ["product_code"] = "BTC_JPY" },
                BodyText = null,
            },
            Response = new ProtocolResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string[]> { ["content-type"] = ["application/json"] },
                BodyText = """{"product_code":"BTC_JPY"}""",
            },
            IsSuccess = true,
            Error = null,
            Meta = new CallMeta
            {
                Layer = CallLayers.Protocol,
                Component = CallComponents.PublicEndpointModule,
                EndpointId = "GetTicker",
                Scope = "Public",
                Auth = "None",
                Children = null,
            },
        };

        var outcome = ExecutionOutcome.FromProtocolCall(
            new CommandPath("bitflyer", "protocol", "public", "get-ticker"),
            call);

        Assert.Equal(CliExitCode.Success, outcome.ExitCode);
        var envelope = Assert.IsType<ProtocolCallEnvelope>(outcome.Response);
        Assert.Equal("/v1/getticker", envelope.Request.Path);
        Assert.Equal(200, envelope.Response.StatusCode);
        Assert.Equal("GetTicker", envelope.Meta.EndpointId);
    }

    [Fact]
    public async Task WizardGetTicker_PrintsCanonicalRequestJsonCommand()
    {
        var console = new FakeConsole();
        console.EnqueueInputLine("BTC_JPY");
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["wizard", "bitflyer", "native", "public", "get-ticker"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal(
            """exchangeapi bitflyer native public get-ticker --request-json '{"product_code":"BTC_JPY"}'""" + Environment.NewLine,
            console.StdOut);
        Assert.Contains("Wizard: bitflyer native public get-ticker", console.StdErr);
    }

    [Fact]
    public async Task WizardCancelAllChildOrders_PrintsSafetyNote()
    {
        var console = new FakeConsole();
        console.EnqueueInputLine("BTC_JPY");
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["wizard", "bitflyer", "native", "private", "cancel-all-child-orders"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal(
            """exchangeapi bitflyer native private cancel-all-child-orders --request-json '{"product_code":"BTC_JPY"}'""" + Environment.NewLine,
            console.StdOut);
        Assert.Contains("write command note", console.StdErr);
    }

    [Fact]
    public async Task ShellHelp_PrintsSupportedBuiltIns()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["shell", "--help"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("exchangeapi shell", console.StdErr);
        Assert.Contains("use venue <value>", console.StdErr);
        Assert.Contains("run <command> [options]", console.StdErr);
    }

    [Fact]
    public async Task ShellRunWithDefaults_DelegatesToCanonicalCommand()
    {
        var console = new FakeConsole();
        console.EnqueueInputLine("use venue fake");
        console.EnqueueInputLine("use surface native");
        console.EnqueueInputLine("use scope public");
        console.EnqueueInputLine("show");
        console.EnqueueInputLine("run echo --summary --pretty");
        console.EnqueueInputLine("quit");

        var app = new CliApplication(
            commands:
            [
                new CommandDescriptor
                {
                    Path = new CommandPath("fake", "native", "public", "echo"),
                    EndpointId = "Echo",
                    Summary = "fake command",
                    AuthenticationRequirement = "none",
                    InputMode = CommandInputMode.NativeRequest,
                    CanonicalJsonExample = "exchangeapi fake native public echo",
                    TemplateJson = "{}",
                    CommandOptions = [],
                    UsageExamples = ["exchangeapi fake native public echo --summary"],
                    IsWrite = false,
                    BindRequestAsync = static (_, _, _) => Task.FromResult(RequestBindingResult.Success(new object())),
                    DescribeRequest = static _ => "none",
                    ExecuteAsync = static (_, _, _, _) => Task.FromResult(
                        ExecutionOutcome.Success(
                            "fake native public echo: success",
                            new EchoResponse { Message = "ok" })),
                },
            ],
            console: console,
            environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["shell"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("Shell helper started.", console.StdErr);
        Assert.Contains("venue=fake surface=native scope=public", console.StdErr);
        Assert.Contains("shell executed: fake native public echo", console.StdErr);
        Assert.Contains("fake native public echo: success", console.StdErr);
        Assert.Contains("\"Message\": \"ok\"", console.StdOut);
    }

    [Fact]
    public async Task ShellRunFailure_PrintsExitCodeAndPreservesFailureMeaning()
    {
        var console = new FakeConsole();
        console.EnqueueInputLine("use venue fake");
        console.EnqueueInputLine("use surface native");
        console.EnqueueInputLine("use scope public");
        console.EnqueueInputLine("run echo --unknown");
        console.EnqueueInputLine("show");
        console.EnqueueInputLine("quit");

        var app = new CliApplication(
            commands:
            [
                new CommandDescriptor
                {
                    Path = new CommandPath("fake", "native", "public", "echo"),
                    EndpointId = "Echo",
                    Summary = "fake command",
                    AuthenticationRequirement = "none",
                    InputMode = CommandInputMode.NativeRequest,
                    CanonicalJsonExample = "exchangeapi fake native public echo",
                    TemplateJson = "{}",
                    CommandOptions = [],
                    UsageExamples = ["exchangeapi fake native public echo --summary"],
                    IsWrite = false,
                    BindRequestAsync = static (_, _, _) => Task.FromResult(RequestBindingResult.Success(new object())),
                    DescribeRequest = static _ => "none",
                    ExecuteAsync = static (_, _, _, _) => Task.FromResult(
                        ExecutionOutcome.Success(
                            "fake native public echo: success",
                            new EchoResponse { Message = "ok" })),
                },
            ],
            console: console,
            environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["shell"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("invalid option", console.StdErr);
        Assert.Contains("unknown option: --unknown", console.StdErr);
        Assert.Contains("shell failed: fake native public echo exit=2", console.StdErr);
        Assert.Contains("last-exit-code=2", console.StdErr);
    }

    private sealed class EchoResponse
    {
        public required string Message { get; init; }
    }
}
