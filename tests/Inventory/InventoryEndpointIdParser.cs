using System;
using System.Collections.Generic;
using System.IO;

namespace ExchangeApi.Tests.Inventory;

internal static class InventoryEndpointIdParser
{
    private const int EndpointIdColumnIndex = 5;
    private const int PresentInColumnIndex = 6;

    public static HashSet<string> ParseEndpointIdsFromFile(string path)
    {
        var text = File.ReadAllText(path);
        return ParseEndpointIds(text);
    }

    public static HashSet<string> ParseEndpointIds(string markdown)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        var lines = markdown.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        foreach (var line in lines)
        {
            if (!line.StartsWith("|", StringComparison.Ordinal))
            {
                continue;
            }

            var trimmed = line.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            var cells = trimmed.Split('|');
            if (cells.Length <= EndpointIdColumnIndex)
            {
                continue;
            }

            var endpointId = cells[EndpointIdColumnIndex].Trim();
            if (endpointId.Length == 0)
            {
                continue;
            }

            if (string.Equals(endpointId, "EndpointId", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (IsSeparatorToken(endpointId))
            {
                continue;
            }

            if (cells.Length > PresentInColumnIndex)
            {
                var presentIn = cells[PresentInColumnIndex].Trim();
                if (string.Equals(presentIn, "None", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            ids.Add(endpointId);
        }

        return ids;
    }

    public static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "ExchangeApi.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Repository root not found (ExchangeApi.slnx missing).");
    }

    private static bool IsSeparatorToken(string value)
    {
        foreach (var ch in value)
        {
            if (ch == '-' || ch == ':' || char.IsWhiteSpace(ch))
            {
                continue;
            }

            return false;
        }

        return true;
    }
}
