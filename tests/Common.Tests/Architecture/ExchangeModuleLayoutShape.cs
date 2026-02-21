using System;
using System.IO;
using System.Text.Json;

namespace Common.Tests.Architecture;

internal sealed class ExchangeModuleLayoutShape
{
    public DirectoryRule ExchangeRoot { get; set; } = new();

    public DirectoryRule[] DirectoryRules { get; set; } = Array.Empty<DirectoryRule>();

    public string[] RequiredFiles { get; set; } = Array.Empty<string>();

    public string[] AdapterRequiredFiles { get; set; } = Array.Empty<string>();

    public string AdapterMapPattern { get; set; } = string.Empty;

    public ConditionalRule[] ConditionalRules { get; set; } = Array.Empty<ConditionalRule>();

    public static ExchangeModuleLayoutShape Load(string repoRoot)
    {
        var path = Path.Combine(
            repoRoot,
            "docs",
            "normative",
            "layout",
            "exchange-module-shape.json");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Exchange module layout shape file not found.", path);
        }

        using var stream = File.OpenRead(path);
        var shape = JsonSerializer.Deserialize<ExchangeModuleLayoutShape>(
            stream,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        if (shape is null)
        {
            throw new InvalidDataException($"Failed to deserialize layout shape: {path}");
        }

        return shape;
    }

    public static string ExpandPathTemplate(string template, string exchange) =>
        template
            .Replace("{Exchange}", exchange, StringComparison.Ordinal)
            .Replace('/', Path.DirectorySeparatorChar);
}

internal sealed class DirectoryRule
{
    public string Path { get; set; } = ".";

    public string[] RequiredDirectories { get; set; } = Array.Empty<string>();

    public string[] OptionalDirectories { get; set; } = Array.Empty<string>();

    public string[] ForbiddenDirectories { get; set; } = Array.Empty<string>();

    public bool AllowFiles { get; set; } = true;
}

internal sealed class ConditionalRule
{
    public string Name { get; set; } = string.Empty;

    public string WhenDirectory { get; set; } = string.Empty;

    public bool WhenHasCsFiles { get; set; }

    public string[] RequiredDirectories { get; set; } = Array.Empty<string>();

    public string[] RequiredFiles { get; set; } = Array.Empty<string>();
}
