using System;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.IO;
using System.Linq;
using Xunit;

namespace Common.Tests.Architecture;

public sealed class ExchangeModuleLayoutParityTests
{
    [Fact]
    public void ExchangeModules_ShouldMatchCanonicalDirectoryShape()
    {
        var root = FindRepoRoot();
        var shape = ExchangeModuleLayoutShape.Load(root);
        var exchangesRoot = Path.Combine(root, "src", "Exchanges");
        Assert.True(Directory.Exists(exchangesRoot));

        var exchanges = Directory
            .GetDirectories(exchangesRoot)
            .Select(static path => Path.GetFileName(path) ?? string.Empty)
            .Where(static name => name.Length > 0)
            .Where(static name => !string.Equals(name, "Common", StringComparison.Ordinal))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(exchanges);

        foreach (var exchange in exchanges)
        {
            var exchangePath = Path.Combine(exchangesRoot, exchange);
            AssertDirectoryShape(
                exchangePath,
                shape.ExchangeRoot.RequiredDirectories,
                shape.ExchangeRoot.OptionalDirectories,
                shape.ExchangeRoot.ForbiddenDirectories,
                shape.ExchangeRoot.AllowFiles!.Value,
                ExpandTemplates(shape.ExchangeRoot.AllowedFiles, exchange),
                ExpandTemplates(shape.ExchangeRoot.AllowedFilePatterns, exchange),
                scope: $"{exchange} root");

            foreach (var rule in shape.DirectoryRules)
            {
                var targetPath = string.Equals(rule.Path, ".", StringComparison.Ordinal)
                    ? exchangePath
                    : Path.Combine(exchangePath, ExchangeModuleLayoutShape.ExpandPathTemplate(rule.Path, exchange));
                AssertDirectoryShape(
                    targetPath,
                    rule.RequiredDirectories,
                    rule.OptionalDirectories,
                    rule.ForbiddenDirectories,
                    rule.AllowFiles!.Value,
                    ExpandTemplates(rule.AllowedFiles, exchange),
                    ExpandTemplates(rule.AllowedFilePatterns, exchange),
                    scope: $"{exchange}/{rule.Path}");
            }

            foreach (var template in shape.RequiredFiles)
            {
                var filePath = Path.Combine(exchangePath, ExchangeModuleLayoutShape.ExpandPathTemplate(template, exchange));
                Assert.True(File.Exists(filePath), $"Missing required file: {filePath}");
            }

            foreach (var conditional in shape.ConditionalRules)
            {
                var whenDirectoryPath = Path.Combine(
                    exchangePath,
                    ExchangeModuleLayoutShape.ExpandPathTemplate(conditional.WhenDirectory, exchange));
                if (!Directory.Exists(whenDirectoryPath))
                {
                    continue;
                }

                var hasCsFiles = Directory.GetFiles(whenDirectoryPath, "*.cs", SearchOption.TopDirectoryOnly).Length > 0;
                if (conditional.WhenHasCsFiles && !hasCsFiles)
                {
                    continue;
                }

                foreach (var requiredDirectory in conditional.RequiredDirectories)
                {
                    var requiredDirectoryPath = Path.Combine(
                        exchangePath,
                        ExchangeModuleLayoutShape.ExpandPathTemplate(requiredDirectory, exchange));
                    Assert.True(Directory.Exists(requiredDirectoryPath), $"Missing required directory ({conditional.Name}): {requiredDirectoryPath}");
                }

                foreach (var requiredFile in conditional.RequiredFiles)
                {
                    var requiredFilePath = Path.Combine(
                        exchangePath,
                        ExchangeModuleLayoutShape.ExpandPathTemplate(requiredFile, exchange));
                    Assert.True(File.Exists(requiredFilePath), $"Missing required file ({conditional.Name}): {requiredFilePath}");
                }
            }
        }
    }

    private static void AssertDirectoryShape(
        string basePath,
        IReadOnlyCollection<string> requiredNames,
        IReadOnlyCollection<string> optionalNames,
        IReadOnlyCollection<string> forbiddenNames,
        bool allowFiles,
        IReadOnlyCollection<string> allowedFiles,
        IReadOnlyCollection<string> allowedFilePatterns,
        string scope)
    {
        Assert.True(Directory.Exists(basePath), $"Directory not found: {basePath}");

        var children = Directory
            .GetDirectories(basePath)
            .Select(static path => Path.GetFileName(path) ?? string.Empty)
            .Where(static name => name.Length > 0)
            .Where(static name => !IsBuildArtifactDirectory(name))
            .ToHashSet(StringComparer.Ordinal);

        var allowed = requiredNames
            .Concat(optionalNames)
            .ToHashSet(StringComparer.Ordinal);

        var missing = requiredNames
            .Where(name => !children.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var unexpected = children
            .Where(name => !allowed.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var forbidden = children
            .Where(name => forbiddenNames.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0,
            $"Missing required directories in {scope}: {string.Join(", ", missing)}");
        Assert.True(
            unexpected.Length == 0,
            $"Unexpected directories in {scope}: {string.Join(", ", unexpected)}");
        Assert.True(
            forbidden.Length == 0,
            $"Forbidden directories in {scope}: {string.Join(", ", forbidden)}");

        if (!allowFiles)
        {
            var files = Directory
                .GetFiles(basePath, "*", SearchOption.TopDirectoryOnly)
                .Select(static path => Path.GetFileName(path) ?? string.Empty)
                .Where(static name => name.Length > 0)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToArray();
            var unexpectedFiles = files
                .Where(file => !IsAllowedFile(file, allowedFiles, allowedFilePatterns))
                .ToArray();
            Assert.True(
                unexpectedFiles.Length == 0,
                $"Unexpected files in {scope}: {string.Join(", ", unexpectedFiles)}");
        }
    }

    private static string[] ExpandTemplates(IEnumerable<string> templates, string exchange) =>
        templates
            .Select(template => ExchangeModuleLayoutShape.ExpandPathTemplate(template, exchange))
            .ToArray();

    private static bool IsAllowedFile(
        string fileName,
        IReadOnlyCollection<string> allowedFiles,
        IReadOnlyCollection<string> allowedFilePatterns)
    {
        if (allowedFiles.Contains(fileName))
        {
            return true;
        }

        foreach (var pattern in allowedFilePatterns)
        {
            if (FileSystemName.MatchesSimpleExpression(pattern, fileName, ignoreCase: false))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsBuildArtifactDirectory(string name) =>
        string.Equals(name, "bin", StringComparison.Ordinal) ||
        string.Equals(name, "obj", StringComparison.Ordinal);

    private static string FindRepoRoot()
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
}
