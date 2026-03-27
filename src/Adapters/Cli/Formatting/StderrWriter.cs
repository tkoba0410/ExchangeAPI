using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Formatting;

public static class StderrWriter
{
    public static void WriteFailure(IConsole console, ExecutionOutcome outcome, bool verbose)
    {
        console.WriteErrorLine(outcome.Summary);

        if (!string.IsNullOrWhiteSpace(outcome.Detail))
        {
            console.WriteErrorLine(outcome.Detail);
        }

        if (!verbose)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(outcome.ErrorKind))
        {
            console.WriteErrorLine($"CallError.Kind: {outcome.ErrorKind}");
        }

        if (!string.IsNullOrWhiteSpace(outcome.EndpointId))
        {
            console.WriteErrorLine($"EndpointId: {outcome.EndpointId}");
        }

        if (!string.IsNullOrWhiteSpace(outcome.ProtocolPath))
        {
            console.WriteErrorLine($"ProtocolPath: {outcome.ProtocolPath}");
        }

        if (outcome.ProtocolStatusCode is not null)
        {
            console.WriteErrorLine($"ProtocolStatusCode: {outcome.ProtocolStatusCode.Value}");
        }
    }

    public static void WriteSuccessSummary(IConsole console, string summary)
    {
        console.WriteErrorLine(summary);
    }
}
