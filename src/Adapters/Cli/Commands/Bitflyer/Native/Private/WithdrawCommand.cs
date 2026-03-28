using ExchangeApi.Adapters.Cli.Binding;
using ExchangeApi.Adapters.Cli.Configuration;
using ExchangeApi.Adapters.Cli.Infrastructure;
using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;

namespace ExchangeApi.Adapters.Cli.Commands.Bitflyer.Native.Private;

public static class WithdrawCommand
{
    public static CommandDescriptor Create()
    {
        return new CommandDescriptor
        {
            Path = new CommandPath("bitflyer", "native", "private", "withdraw"),
            EndpointId = "Withdraw",
            Summary = "bitFlyer native private withdraw",
            AuthenticationRequirement = "BITFLYER_API_KEY / BITFLYER_API_SECRET",
            InputContract = CommandInputContract.NativeRequest("""{"currency_code":"JPY","bank_account_id":0,"amount":0,"code":""}"""),
            CanonicalJsonExample = """exchangeapi bitflyer native private withdraw --request-json '{"currency_code":"JPY","bank_account_id":12345,"amount":10000,"code":"123456"}' --yes""",
            CommandOptions =
            [
                CliOptionSpec.Value("currency-code"),
                CliOptionSpec.Value("bank-account-id", "long"),
                CliOptionSpec.Value("amount", "decimal"),
                CliOptionSpec.Value("code"),
            ],
            UsageExamples =
            [
                "exchangeapi bitflyer native private withdraw --currency-code JPY --bank-account-id 12345 --amount 10000 --code 123456 --yes",
                """exchangeapi bitflyer native private withdraw --request-json '{"currency_code":"JPY","bank_account_id":12345,"amount":10000,"code":"123456"}' --yes""",
                "exchangeapi bitflyer native private withdraw --request-template",
            ],
            IsWrite = true,
            BindRequestAsync = BindRequestAsync,
            DescribeRequest = static request =>
            {
                var typed = (WithdrawRequest)request;
                return $"currency_code={typed.CurrencyCode}, bank_account_id={typed.BankAccountId}, amount={typed.Amount}";
            },
            ExecuteAsync = ExecuteAsync,
        };
    }

    private static async Task<RequestBindingResult> BindRequestAsync(
        InvocationOptions options,
        IConsole console,
        CancellationToken cancellationToken)
    {
        var hasConvenience = options.Contains("currency-code")
            || options.Contains("bank-account-id")
            || options.Contains("amount")
            || options.Contains("code");

        var jsonInput = await JsonInputReader.ReadTextAsync(options, "request-json", "request-file", console, cancellationToken);
        if (jsonInput.Failure is not null)
        {
            return jsonInput.Failure;
        }

        if (jsonInput.HasValue && hasConvenience)
        {
            return RequestBindingResult.Failure(
                "invalid argument",
                "--request-json/--request-file and convenience flags cannot be used together");
        }

        if (jsonInput.HasValue)
        {
            return JsonInputReader.Deserialize<WithdrawRequest>(jsonInput.Content!);
        }

        if (!OptionValueBinder.TryGetRequiredString(options, "currency-code", "currency_code", out var currencyCode, out var currencyCodeError))
        {
            return RequestBindingResult.Failure("invalid argument", currencyCodeError);
        }

        if (!OptionValueBinder.TryGetRequiredLong(options, "bank-account-id", "bank_account_id", out var bankAccountId, out var bankAccountIdError))
        {
            return RequestBindingResult.Failure("invalid argument", bankAccountIdError);
        }

        if (!OptionValueBinder.TryGetRequiredDecimal(options, "amount", "amount", out var amount, out var amountError))
        {
            return RequestBindingResult.Failure("invalid argument", amountError);
        }

        if (!OptionValueBinder.TryGetRequiredString(options, "code", "code", out var code, out var codeError))
        {
            return RequestBindingResult.Failure("invalid argument", codeError);
        }

        return RequestBindingResult.Success(new WithdrawRequest
        {
            CurrencyCode = currencyCode,
            BankAccountId = bankAccountId,
            Amount = amount,
            Code = code,
        });
    }

    private static async Task<ExecutionOutcome> ExecuteAsync(
        InvocationOptions options,
        object request,
        IEnvironment environment,
        CancellationToken cancellationToken)
    {
        var created = BitflyerOptionsFactory.Create(options, environment, requiresCredentials: true);
        if (created.Failure is not null)
        {
            return created.Failure;
        }

        using var bundle = BitflyerClientFactory.CreateNativeClient(created.Options);
        if (bundle.Private is null)
        {
            return ExecutionOutcome.InputError(
                "missing credential",
                "BITFLYER_API_KEY and BITFLYER_API_SECRET must be set");
        }

        var call = await bundle.Private.WithdrawCallAsync((WithdrawRequest)request, cancellationToken);
        return ExecutionOutcome.FromCall(new CommandPath("bitflyer", "native", "private", "withdraw"), call);
    }
}
