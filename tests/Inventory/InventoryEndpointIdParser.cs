using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExchangeApi.Tests.Inventory;

internal static class InventoryEndpointIdParser
{
    private const string EndpointIdColumnName = "EndpointId";
    private const string PresentInColumnName = "PresentIn";

    public static HashSet<string> ParseEndpointIdsFromFile(string path)
    {
        var text = File.ReadAllText(path);
        return ParseEndpointIds(text);
    }

    public static HashSet<string> ParseEndpointIds(string markdown)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);

        var lines = markdown.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (!line.TrimStart().StartsWith("|", StringComparison.Ordinal))
            {
                continue;
            }

            var header = SplitRow(line);
            if (!TryResolveHeader(header, out var endpointIdColumnIndex, out var presentInColumnIndex))
            {
                continue;
            }

            if (i + 1 >= lines.Length || !IsSeparatorRow(SplitRow(lines[i + 1])))
            {
                continue;
            }

            for (var j = i + 2; j < lines.Length; j++)
            {
                var rowLine = lines[j];
                if (!rowLine.TrimStart().StartsWith("|", StringComparison.Ordinal))
                {
                    i = j - 1;
                    break;
                }

                var row = SplitRow(rowLine);
                if (row.Count == 0 || IsSeparatorRow(row) || row.Count <= endpointIdColumnIndex)
                {
                    continue;
                }

                var endpointId = row[endpointIdColumnIndex];
                if (endpointId.Length == 0 || IsSeparatorToken(endpointId))
                {
                    continue;
                }

                if (presentInColumnIndex >= 0 && row.Count > presentInColumnIndex)
                {
                    var presentIn = row[presentInColumnIndex];
                    if (string.Equals(presentIn, "None", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }

                ids.Add(endpointId);
                i = j;
            }
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

    private static List<string> SplitRow(string row)
    {
        if (string.IsNullOrWhiteSpace(row))
        {
            return new List<string>();
        }

        return row.Trim()
            .Trim('|')
            .Split('|', StringSplitOptions.None)
            .Select(cell => cell.Trim())
            .ToList();
    }

    private static bool IsSeparatorRow(IReadOnlyList<string> cells)
    {
        foreach (var cell in cells)
        {
            if (!IsSeparatorToken(cell))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryResolveHeader(
        IReadOnlyList<string> cells,
        out int endpointIdColumnIndex,
        out int presentInColumnIndex)
    {
        endpointIdColumnIndex = IndexOfColumn(cells, EndpointIdColumnName);
        if (endpointIdColumnIndex < 0)
        {
            presentInColumnIndex = -1;
            return false;
        }

        presentInColumnIndex = IndexOfColumn(cells, PresentInColumnName);
        return true;
    }

    private static int IndexOfColumn(IReadOnlyList<string> cells, string name)
    {
        for (var i = 0; i < cells.Count; i++)
        {
            if (string.Equals(cells[i], name, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return -1;
    }
}
