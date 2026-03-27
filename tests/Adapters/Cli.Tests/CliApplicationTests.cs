using ExchangeApi.Adapters.Cli.Infrastructure;

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
                    CanonicalJsonExample = "exchangeapi fake native public echo",
                    TemplateJson = "{}",
                    ConvenienceFlags = [],
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

    private sealed class EchoResponse
    {
        public required string Message { get; init; }
    }
}
