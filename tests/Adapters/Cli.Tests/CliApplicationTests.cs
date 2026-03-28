using System.Text.Json;
using System.Text;
using ExchangeApi.Adapters.Cli.Commands;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;
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
    public async Task GetTradingCommissionTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-trading-commission", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":""}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetBalanceHistoryTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-balance-history", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"currency_code":null,"count":null,"before":null,"after":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetParentOrderTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-parent-order", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"parent_order_id":null,"parent_order_acceptance_id":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task SendParentOrderTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "send-parent-order", "--request-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"order_method":"SIMPLE","minute_to_expire":null,"time_in_force":null,"parameters":[{"product_code":"","condition_type":"","side":"","price":null,"size":0,"trigger_price":null,"offset":null}]}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetExecutionsPrivateProtocolTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "private", "get-executions-private", "--query-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":null,"count":null,"before":null,"after":null,"child_order_id":null,"child_order_acceptance_id":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetParentOrderProtocolTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "private", "get-parent-order", "--query-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"parent_order_id":null,"parent_order_acceptance_id":null}""", console.StdOut);
        Assert.Equal(string.Empty, console.StdErr);
    }

    [Fact]
    public async Task GetFundingRateProtocolTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "public", "get-funding-rate", "--query-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":null}""", console.StdOut);
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
    public async Task GetExecutionsPublicProtocolTemplate_PrintsCanonicalTemplate()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "public", "get-executions-public", "--query-template"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Equal("""{"product_code":null,"count":null,"before":null,"after":null}""", console.StdOut);
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
    public async Task GetPositionsWithoutProductCode_FailsInputValidation()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-positions"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: product_code", console.StdErr);
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
    public async Task GetExecutionsPrivateRejectsInvalidCountValue()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-executions-private", "--product-code", "BTC_JPY", "--count", "abc"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: count", console.StdErr);
    }

    [Fact]
    public async Task GetWithdrawalsRejectsInvalidBeforeValue()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-withdrawals", "--before", "abc"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: before", console.StdErr);
    }

    [Fact]
    public async Task GetCoinInsRejectsInvalidCountValue()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-coin-ins", "--count", "abc"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: count", console.StdErr);
    }

    [Fact]
    public async Task GetParentOrderRequiresExactlyOneIdentifier()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "get-parent-order"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("exactly one of parent_order_id or parent_order_acceptance_id must be specified", console.StdErr);
    }

    [Fact]
    public async Task CancelChildOrderRequiresExactlyOneIdentifier()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "cancel-child-order", "--product-code", "BTC_JPY"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("exactly one of child_order_id or child_order_acceptance_id must be specified", console.StdErr);
    }

    [Fact]
    public async Task WithdrawRejectsInvalidBankAccountId()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "withdraw", "--currency-code", "JPY", "--bank-account-id", "abc", "--amount", "1000", "--code", "123456"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: bank_account_id", console.StdErr);
    }

    [Fact]
    public async Task SendParentOrderRequiresJsonInputInCurrentPhase()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "native", "private", "send-parent-order"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("send-parent-order requires --request-json or --request-file in the current phase", console.StdErr);
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
    public async Task RejectsProtocolBodyOptionInCurrentPhase()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "public", "get-ticker", "--body-json", "{}"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid option", console.StdErr);
        Assert.Contains("unknown option: --body-json", console.StdErr);
    }

    [Fact]
    public async Task BinanceProtocolKlines_RequiresSymbolAndInterval()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["binance", "protocol", "public", "get-klines", "--query-json", """{"interval":"1h"}"""]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("invalid field: symbol", console.StdErr);
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
        Assert.Contains("Query fields:", console.StdOut);
        Assert.Contains("product_code <string> optional", console.StdOut);
    }

    [Fact]
    public async Task BinanceProtocolHelp_PrintsQueryFieldSchema()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["binance", "protocol", "public", "get-klines", "--help"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("Query fields:", console.StdOut);
        Assert.Contains("symbol <string> required", console.StdOut);
        Assert.Contains("interval <string> required", console.StdOut);
        Assert.Contains("limit <int> optional", console.StdOut);
    }

    [Fact]
    public async Task BitflyerPrivateProtocolHelp_PrintsQueryFieldSchema()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "private", "get-child-orders", "--help"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("Query fields:", console.StdOut);
        Assert.Contains("product_code <string> optional", console.StdOut);
        Assert.Contains("child_order_state <string> optional", console.StdOut);
        Assert.Contains("parent_order_id <string> optional", console.StdOut);
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
    public async Task PrivateProtocolCommand_RequiresCredentials()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "private", "get-permissions", "--query-json", "{}"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("missing credential", console.StdErr);
        Assert.Contains("BITFLYER_API_KEY", console.StdErr);
    }

    [Fact]
    public async Task ParentOrderProtocolRequiresExactlyOneIdentifier()
    {
        var console = new FakeConsole();
        var app = new CliApplication(console: console, environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(
            ["bitflyer", "protocol", "private", "get-parent-order", "--query-json", "{}"]);

        Assert.Equal(CliExitCode.ArgumentConfigOrSafetyError, exitCode);
        Assert.Contains("invalid argument", console.StdErr);
        Assert.Contains("exactly one of parent_order_id or parent_order_acceptance_id must be specified", console.StdErr);
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
                    InputContract = CommandInputContract.NativeRequest("{}"),
                    CanonicalJsonExample = "exchangeapi fake native public echo",
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
                "bitflyer native private cancel-child-order",
                "bitflyer native private cancel-parent-order",
                "bitflyer native private get-addresses",
                "bitflyer native private get-balance",
                "bitflyer native private get-balance-history",
                "bitflyer native private get-bank-accounts",
                "bitflyer native private get-child-orders",
                "bitflyer native private get-coin-ins",
                "bitflyer native private get-coin-outs",
                "bitflyer native private get-collateral",
                "bitflyer native private get-collateral-accounts",
                "bitflyer native private get-collateral-history",
                "bitflyer native private get-deposits",
                "bitflyer native private get-executions-private",
                "bitflyer native private get-parent-order",
                "bitflyer native private get-parent-orders",
                "bitflyer native private get-permissions",
                "bitflyer native private get-positions",
                "bitflyer native private get-trading-commission",
                "bitflyer native private get-withdrawals",
                "bitflyer native private send-child-order",
                "bitflyer native private send-parent-order",
                "bitflyer native private withdraw",
                "bitflyer native public get-board",
                "bitflyer native public get-board-state",
                "bitflyer native public get-chats",
                "bitflyer native public get-corporate-leverage",
                "bitflyer native public get-executions-public",
                "bitflyer native public get-funding-rate",
                "bitflyer native public get-health",
                "bitflyer native public get-markets",
                "bitflyer native public get-ticker",
                "bitflyer protocol private get-addresses",
                "bitflyer protocol private get-balance",
                "bitflyer protocol private get-balance-history",
                "bitflyer protocol private get-bank-accounts",
                "bitflyer protocol private get-child-orders",
                "bitflyer protocol private get-coin-ins",
                "bitflyer protocol private get-coin-outs",
                "bitflyer protocol private get-collateral",
                "bitflyer protocol private get-collateral-accounts",
                "bitflyer protocol private get-collateral-history",
                "bitflyer protocol private get-deposits",
                "bitflyer protocol private get-executions-private",
                "bitflyer protocol private get-parent-order",
                "bitflyer protocol private get-parent-orders",
                "bitflyer protocol private get-permissions",
                "bitflyer protocol private get-positions",
                "bitflyer protocol private get-trading-commission",
                "bitflyer protocol private get-withdrawals",
                "bitflyer protocol public get-board",
                "bitflyer protocol public get-board-state",
                "bitflyer protocol public get-chats",
                "bitflyer protocol public get-corporate-leverage",
                "bitflyer protocol public get-executions-public",
                "bitflyer protocol public get-funding-rate",
                "bitflyer protocol public get-health",
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
                ("bitflyer", "protocol", "private"),
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
                "cancel-child-order",
                "cancel-parent-order",
                "get-addresses",
                "get-balance",
                "get-balance-history",
                "get-bank-accounts",
                "get-child-orders",
                "get-coin-ins",
                "get-coin-outs",
                "get-collateral",
                "get-collateral-accounts",
                "get-collateral-history",
                "get-deposits",
                "get-executions-private",
                "get-parent-order",
                "get-parent-orders",
                "get-permissions",
                "get-positions",
                "get-trading-commission",
                "get-withdrawals",
                "send-child-order",
                "send-parent-order",
                "withdraw",
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
                "get-addresses",
                "get-balance",
                "get-balance-history",
                "get-bank-accounts",
                "get-child-orders",
                "get-coin-ins",
                "get-coin-outs",
                "get-collateral",
                "get-collateral-accounts",
                "get-collateral-history",
                "get-deposits",
                "get-executions-private",
                "get-parent-order",
                "get-parent-orders",
                "get-permissions",
                "get-positions",
                "get-trading-commission",
                "get-withdrawals",
            ],
            grouped[("bitflyer", "protocol", "private")]);

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
    public void BitflyerNativePublic_CommandCoverage_MatchesInterfaceSurface()
    {
        AssertInterfaceMethodsCovered<IBitflyerPublicNativeApi>(
            "bitflyer",
            "native",
            "public");
    }

    [Fact]
    public void BitflyerNativePrivate_CommandCoverage_MatchesInterfaceSurface()
    {
        AssertInterfaceMethodsCovered<IBitflyerPrivateNativeApi>(
            "bitflyer",
            "native",
            "private");
    }

    [Fact]
    public void BitflyerProtocolPublic_CommandCoverage_MatchesInterfaceSurface()
    {
        AssertInterfaceMethodsCovered<IBitflyerPublicProtocolApi>(
            "bitflyer",
            "protocol",
            "public");
    }

    [Fact]
    public void BitflyerProtocolPrivate_CommandCoverage_MatchesCurrentPhaseReadSurface()
    {
        AssertInterfaceMethodsCovered<IBitflyerPrivateProtocolApi>(
            "bitflyer",
            "protocol",
            "private",
            static method => method.GetParameters().All(static p => !string.Equals(p.Name, "bodyJson", StringComparison.Ordinal)));
    }

    [Fact]
    public void BinanceNativePublic_CommandCoverage_MatchesInterfaceSurface()
    {
        AssertInterfaceMethodsCovered<IBinancePublicNativeApi>(
            "binance",
            "native",
            "public");
    }

    [Fact]
    public void BinanceProtocolPublic_CommandCoverage_MatchesInterfaceSurface()
    {
        AssertInterfaceMethodsCovered<IBinancePublicProtocolApi>(
            "binance",
            "protocol",
            "public");
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
        Assert.Equal("bitflyer protocol public get-ticker: success (status=200)", outcome.Summary);
        var envelope = Assert.IsType<ProtocolCallEnvelope>(outcome.Response);
        Assert.Equal("/v1/getticker", envelope.Request.Path);
        Assert.Equal(200, envelope.Response.StatusCode);
        Assert.Equal("GetTicker", envelope.Meta.EndpointId);
    }

    [Fact]
    public async Task InjectedProtocolCommand_SummaryIncludesStatusCode()
    {
        var console = new FakeConsole();
        var app = new CliApplication(
            commands:
            [
                new CommandDescriptor
                {
                    Path = new CommandPath("fake", "protocol", "public", "echo"),
                    EndpointId = "Echo",
                    Summary = "fake protocol command",
                    AuthenticationRequirement = "none",
                    InputContract = CommandInputContract.ProtocolQuery(new ProtocolQuerySchema([])),
                    CanonicalJsonExample = "exchangeapi fake protocol public echo --query-json '{}'",
                    CommandOptions = [],
                    UsageExamples = ["exchangeapi fake protocol public echo --summary"],
                    IsWrite = false,
                    BindRequestAsync = static (_, _, _) => Task.FromResult(
                        RequestBindingResult.Success(
                            new ProtocolQueryValues(
                                new ProtocolQuerySchema([]),
                                new Dictionary<string, JsonElement>(StringComparer.Ordinal),
                                new Dictionary<string, object?>(StringComparer.Ordinal)))),
                    DescribeRequest = static _ => "query=<none>",
                    ExecuteAsync = static (_, _, _, _) => Task.FromResult(
                        ExecutionOutcome.FromProtocolCall(
                            new CommandPath("fake", "protocol", "public", "echo"),
                            new Call<ProtocolRequest, ProtocolResponse>
                            {
                                Request = new ProtocolRequest
                                {
                                    EndpointId = "Echo",
                                    Method = "GET",
                                    Path = "/echo",
                                    Query = new Dictionary<string, string>(),
                                    BodyText = null,
                                },
                                Response = new ProtocolResponse
                                {
                                    StatusCode = 404,
                                    Headers = new Dictionary<string, string[]>(),
                                    BodyText = "{}",
                                },
                                IsSuccess = true,
                                Error = null,
                                Meta = new CallMeta
                                {
                                    Layer = CallLayers.Protocol,
                                    Component = CallComponents.PublicEndpointModule,
                                    EndpointId = "Echo",
                                    Scope = "Public",
                                    Auth = "None",
                                    Children = null,
                                },
                            })),
                },
            ],
            console: console,
            environment: new FakeEnvironment());

        var exitCode = await app.RunAsync(["fake", "protocol", "public", "echo", "--summary"]);

        Assert.Equal(CliExitCode.Success, exitCode);
        Assert.Contains("fake protocol public echo: success (status=404)", console.StdErr);
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
                    InputContract = CommandInputContract.NativeRequest("{}"),
                    CanonicalJsonExample = "exchangeapi fake native public echo",
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
                    InputContract = CommandInputContract.NativeRequest("{}"),
                    CanonicalJsonExample = "exchangeapi fake native public echo",
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

    private static void AssertInterfaceMethodsCovered<TInterface>(
        string venue,
        string surface,
        string scope,
        Func<System.Reflection.MethodInfo, bool>? include = null)
    {
        include ??= static _ => true;

        var expected = typeof(TInterface)
            .GetMethods()
            .Where(include)
            .Select(method => ToCommandName(method.Name, scope))
            .OrderBy(static x => x, StringComparer.Ordinal)
            .ToArray();

        var actual = CommandCatalog.All
            .Where(x =>
                string.Equals(x.Path.Venue, venue, StringComparison.Ordinal) &&
                string.Equals(x.Path.Surface, surface, StringComparison.Ordinal) &&
                string.Equals(x.Path.Scope, scope, StringComparison.Ordinal))
            .Select(static x => x.Path.Command)
            .OrderBy(static x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(expected, actual);
    }

    private static string ToCommandName(string methodName, string scope)
    {
        var stem = methodName.EndsWith("CallAsync", StringComparison.Ordinal)
            ? methodName[..^"CallAsync".Length]
            : methodName;

        if (string.Equals(stem, "GetExecutions", StringComparison.Ordinal))
        {
            return string.Equals(scope, "public", StringComparison.Ordinal)
                ? "get-executions-public"
                : "get-executions-private";
        }

        return ToKebabCase(stem);
    }

    private static string ToKebabCase(string text)
    {
        var builder = new StringBuilder(text.Length + 8);
        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];
            if (char.IsUpper(character) && index > 0)
            {
                builder.Append('-');
            }

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
