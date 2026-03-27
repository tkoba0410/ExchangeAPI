namespace ExchangeApi.Adapters.Cli.Infrastructure;

public static class CliExitCode
{
    public const int Success = 0;
    public const int UnexpectedInternalError = 1;
    public const int ArgumentConfigOrSafetyError = 2;
    public const int FacadeCallFailure = 3;
}
