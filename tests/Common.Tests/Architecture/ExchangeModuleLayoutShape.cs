using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        shape.Validate(path);
        return shape;
    }

    public static string ExpandPathTemplate(string template, string exchange) =>
        template
            .Replace("{Exchange}", exchange, StringComparison.Ordinal)
            .Replace('/', Path.DirectorySeparatorChar);

    private void Validate(string sourcePath)
    {
        ValidateRule(
            ExchangeRoot,
            "exchangeRoot",
            requirePath: false,
            requireRequiredDirectories: true,
            sourcePath);

        if (DirectoryRules.Length == 0)
        {
            throw new InvalidDataException($"layout shape must define at least one directory rule: {sourcePath}");
        }

        var paths = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < DirectoryRules.Length; i++)
        {
            var rule = DirectoryRules[i];
            var location = $"directoryRules[{i}]";
            ValidateRule(
                rule,
                location,
                requirePath: true,
                requireRequiredDirectories: false,
                sourcePath);

            if (!paths.Add(rule.Path))
            {
                throw new InvalidDataException($"Duplicate directory rule path '{rule.Path}' at {location}: {sourcePath}");
            }
        }

        ValidateNonEmptyArray(RequiredFiles, nameof(RequiredFiles), sourcePath);
        ValidateNonEmptyArray(AdapterRequiredFiles, nameof(AdapterRequiredFiles), sourcePath);
        ValidateNonEmptyValue(AdapterMapPattern, nameof(AdapterMapPattern), sourcePath);

        for (var i = 0; i < ConditionalRules.Length; i++)
        {
            var conditional = ConditionalRules[i];
            var location = $"conditionalRules[{i}]";

            ValidateNonEmptyValue(conditional.Name, $"{location}.name", sourcePath);
            ValidateNonEmptyValue(conditional.WhenDirectory, $"{location}.whenDirectory", sourcePath);
            ValidateArrayEntries(conditional.RequiredDirectories, $"{location}.requiredDirectories", sourcePath);
            ValidateArrayEntries(conditional.RequiredFiles, $"{location}.requiredFiles", sourcePath);
        }
    }

    private static void ValidateRule(
        DirectoryRule rule,
        string location,
        bool requirePath,
        bool requireRequiredDirectories,
        string sourcePath)
    {
        if (requirePath)
        {
            ValidateNonEmptyValue(rule.Path, $"{location}.path", sourcePath);
        }

        if (requireRequiredDirectories)
        {
            ValidateNonEmptyArray(rule.RequiredDirectories, $"{location}.requiredDirectories", sourcePath);
        }
        else
        {
            ValidateArrayEntries(rule.RequiredDirectories, $"{location}.requiredDirectories", sourcePath);
        }

        ValidateArrayEntries(rule.OptionalDirectories, $"{location}.optionalDirectories", sourcePath);
        ValidateArrayEntries(rule.ForbiddenDirectories, $"{location}.forbiddenDirectories", sourcePath);

        if (rule.AllowFiles is null)
        {
            throw new InvalidDataException($"Missing required '{location}.allowFiles' value: {sourcePath}");
        }

        EnsureNoSetIntersection(
            rule.RequiredDirectories,
            rule.OptionalDirectories,
            $"{location}.requiredDirectories",
            $"{location}.optionalDirectories",
            sourcePath);
        EnsureNoSetIntersection(
            rule.RequiredDirectories,
            rule.ForbiddenDirectories,
            $"{location}.requiredDirectories",
            $"{location}.forbiddenDirectories",
            sourcePath);
        EnsureNoSetIntersection(
            rule.OptionalDirectories,
            rule.ForbiddenDirectories,
            $"{location}.optionalDirectories",
            $"{location}.forbiddenDirectories",
            sourcePath);
    }

    private static void ValidateNonEmptyArray(string[] values, string location, string sourcePath)
    {
        if (values.Length == 0)
        {
            throw new InvalidDataException($"'{location}' must not be empty: {sourcePath}");
        }

        ValidateArrayEntries(values, location, sourcePath);
    }

    private static void ValidateArrayEntries(string[] values, string location, string sourcePath)
    {
        var entries = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < values.Length; i++)
        {
            var value = values[i];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidDataException($"'{location}[{i}]' must not be empty: {sourcePath}");
            }

            if (!entries.Add(value))
            {
                throw new InvalidDataException($"Duplicate value '{value}' in {location}: {sourcePath}");
            }
        }
    }

    private static void ValidateNonEmptyValue(string value, string location, string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"'{location}' must not be empty: {sourcePath}");
        }
    }

    private static void EnsureNoSetIntersection(
        IEnumerable<string> left,
        IEnumerable<string> right,
        string leftName,
        string rightName,
        string sourcePath)
    {
        var overlap = left.Intersect(right, StringComparer.Ordinal).OrderBy(static x => x, StringComparer.Ordinal).ToArray();
        if (overlap.Length > 0)
        {
            throw new InvalidDataException(
                $"'{leftName}' and '{rightName}' must not overlap ({string.Join(", ", overlap)}): {sourcePath}");
        }
    }
}

internal sealed class DirectoryRule
{
    public string Path { get; set; } = ".";

    public string[] RequiredDirectories { get; set; } = Array.Empty<string>();

    public string[] OptionalDirectories { get; set; } = Array.Empty<string>();

    public string[] ForbiddenDirectories { get; set; } = Array.Empty<string>();

    public bool? AllowFiles { get; set; }
}

internal sealed class ConditionalRule
{
    public string Name { get; set; } = string.Empty;

    public string WhenDirectory { get; set; } = string.Empty;

    public bool WhenHasCsFiles { get; set; }

    public string[] RequiredDirectories { get; set; } = Array.Empty<string>();

    public string[] RequiredFiles { get; set; } = Array.Empty<string>();
}
