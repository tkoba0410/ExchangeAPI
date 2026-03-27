namespace ExchangeApi.Adapters.Cli.Infrastructure;

public sealed class SpecialCommandResult
{
    public required bool Handled { get; init; }
    public int ExitCode { get; init; }

    public static SpecialCommandResult NotHandled()
    {
        return new SpecialCommandResult
        {
            Handled = false,
        };
    }

    public static SpecialCommandResult FromExitCode(int exitCode)
    {
        return new SpecialCommandResult
        {
            Handled = true,
            ExitCode = exitCode,
        };
    }
}
