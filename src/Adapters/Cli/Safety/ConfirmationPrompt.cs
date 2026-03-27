using ExchangeApi.Adapters.Cli.Infrastructure;

namespace ExchangeApi.Adapters.Cli.Safety;

public static class ConfirmationPrompt
{
    public static bool IsInteractive(IConsole console)
    {
        return !console.IsInputRedirected && !console.IsErrorRedirected;
    }

    public static async Task<bool> ConfirmAsync(
        IConsole console,
        CommandDescriptor descriptor,
        object request,
        CancellationToken cancellationToken)
    {
        console.WriteErrorLine($"{descriptor.Path.Identity}: write confirmation required");
        console.WriteErrorLine($"request: {descriptor.DescribeRequest(request)}");
        console.WriteErrorLine("Proceed? [y/N]");
        var answer = await console.ReadLineAsync(cancellationToken);
        return answer is not null
            && (answer.Equals("y", StringComparison.OrdinalIgnoreCase)
                || answer.Equals("yes", StringComparison.OrdinalIgnoreCase));
    }
}
