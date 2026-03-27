using System.Text;

namespace ExchangeApi.Adapters.Cli.Shell;

public static class ShellLineTokenizer
{
    public static bool TryTokenize(string line, out IReadOnlyList<string> tokens, out string? errorDetail)
    {
        var results = new List<string>();
        var current = new StringBuilder();
        char? quote = null;
        var escaping = false;

        foreach (var ch in line)
        {
            if (escaping)
            {
                current.Append(ch);
                escaping = false;
                continue;
            }

            if (ch == '\\')
            {
                escaping = true;
                continue;
            }

            if (quote is not null)
            {
                if (ch == quote)
                {
                    quote = null;
                    continue;
                }

                current.Append(ch);
                continue;
            }

            if (ch is '"' or '\'')
            {
                quote = ch;
                continue;
            }

            if (char.IsWhiteSpace(ch))
            {
                FlushToken(results, current);
                continue;
            }

            current.Append(ch);
        }

        if (escaping)
        {
            tokens = [];
            errorDetail = "shell line ends with an incomplete escape";
            return false;
        }

        if (quote is not null)
        {
            tokens = [];
            errorDetail = "shell line contains an unterminated quoted string";
            return false;
        }

        FlushToken(results, current);
        tokens = results;
        errorDetail = null;
        return true;
    }

    private static void FlushToken(List<string> results, StringBuilder current)
    {
        if (current.Length == 0)
        {
            return;
        }

        results.Add(current.ToString());
        current.Clear();
    }
}
