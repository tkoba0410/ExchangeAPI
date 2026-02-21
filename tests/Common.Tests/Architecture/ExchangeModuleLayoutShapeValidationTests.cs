using System;
using System.IO;
using System.Text.Json;
using Xunit;

namespace Common.Tests.Architecture;

public sealed class ExchangeModuleLayoutShapeValidationTests
{
    [Fact]
    public void Load_WithValidShape_Succeeds()
    {
        var shape = CreateValidShape();
        var repoRoot = CreateTempRepoWithShape(shape);
        try
        {
            var loaded = ExchangeModuleLayoutShape.Load(repoRoot);
            Assert.NotNull(loaded);
        }
        finally
        {
            Directory.Delete(repoRoot, recursive: true);
        }
    }

    [Fact]
    public void Load_WhenDirectoryRuleRequiredDirectoriesIsEmpty_Throws()
    {
        AssertInvalidShape(
            shape => shape.DirectoryRules[0].RequiredDirectories = Array.Empty<string>(),
            "requiredDirectories");
    }

    [Fact]
    public void Load_WhenDirectoryRulePathIsDuplicated_Throws()
    {
        AssertInvalidShape(
            shape =>
            {
                shape.DirectoryRules = new[]
                {
                    shape.DirectoryRules[0],
                    new DirectoryRule
                    {
                        Path = shape.DirectoryRules[0].Path,
                        RequiredDirectories = new[] { "Other" },
                        AllowFiles = false,
                    },
                };
            },
            "Duplicate directory rule path");
    }

    [Fact]
    public void Load_WhenAllowFilesIsMissing_Throws()
    {
        AssertInvalidShape(
            shape => shape.DirectoryRules[0].AllowFiles = null,
            "allowFiles");
    }

    [Fact]
    public void Load_WhenAllowFilesTrueAndAllowedFilesIsSpecified_Throws()
    {
        AssertInvalidShape(
            shape =>
            {
                shape.DirectoryRules[0].AllowFiles = true;
                shape.DirectoryRules[0].AllowedFiles = new[] { "Any.cs" };
            },
            "must be empty when");
    }

    [Fact]
    public void Load_WhenAllowFilesTrueAndAllowedFilePatternsIsSpecified_Throws()
    {
        AssertInvalidShape(
            shape =>
            {
                shape.DirectoryRules[0].AllowFiles = true;
                shape.DirectoryRules[0].AllowedFilePatterns = new[] { "*.cs" };
            },
            "must be empty when");
    }

    [Fact]
    public void Load_WhenExchangeRootRequiredDirectoriesIsEmpty_Throws()
    {
        AssertInvalidShape(
            shape => shape.ExchangeRoot.RequiredDirectories = Array.Empty<string>(),
            "exchangeRoot.requiredDirectories");
    }

    [Fact]
    public void Load_WhenExchangeRootAllowFilesIsMissing_Throws()
    {
        AssertInvalidShape(
            shape => shape.ExchangeRoot.AllowFiles = null,
            "exchangeRoot.allowFiles");
    }

    [Fact]
    public void Load_WhenExchangeRootRequiredAndOptionalDirectoriesOverlap_Throws()
    {
        AssertInvalidShape(
            shape => shape.ExchangeRoot.OptionalDirectories = new[] { "Wire" },
            "must not overlap");
    }

    [Fact]
    public void Load_WhenExchangeRootRequiredAndForbiddenDirectoriesOverlap_Throws()
    {
        AssertInvalidShape(
            shape => shape.ExchangeRoot.ForbiddenDirectories = new[] { "Wire" },
            "must not overlap");
    }

    [Fact]
    public void Load_WhenExchangeRootOptionalAndForbiddenDirectoriesOverlap_Throws()
    {
        AssertInvalidShape(
            shape =>
            {
                shape.ExchangeRoot.OptionalDirectories = new[] { "OptionalA" };
                shape.ExchangeRoot.ForbiddenDirectories = new[] { "OptionalA" };
            },
            "must not overlap");
    }

    [Fact]
    public void Load_WhenConditionalRuleHasNoRequirements_Throws()
    {
        AssertInvalidShape(
            shape =>
            {
                shape.ConditionalRules[0].RequiredDirectories = Array.Empty<string>();
                shape.ConditionalRules[0].RequiredFiles = Array.Empty<string>();
            },
            "must define at least one required directory or file");
    }

    [Fact]
    public void Load_WhenRequiredAndOptionalDirectoriesOverlap_Throws()
    {
        AssertInvalidShape(
            shape => shape.DirectoryRules[0].OptionalDirectories = new[] { "Public" },
            "must not overlap");
    }

    [Fact]
    public void Load_WhenRequiredAndForbiddenDirectoriesOverlap_Throws()
    {
        AssertInvalidShape(
            shape => shape.DirectoryRules[0].ForbiddenDirectories = new[] { "Public" },
            "must not overlap");
    }

    [Fact]
    public void Load_WhenOptionalAndForbiddenDirectoriesOverlap_Throws()
    {
        AssertInvalidShape(
            shape =>
            {
                shape.DirectoryRules[0].OptionalDirectories = new[] { "OptionalA" };
                shape.DirectoryRules[0].ForbiddenDirectories = new[] { "OptionalA" };
            },
            "must not overlap");
    }

    [Fact]
    public void Load_WhenAllowedFilesContainsDuplicates_Throws()
    {
        AssertInvalidShape(
            shape => shape.DirectoryRules[0].AllowedFiles = new[] { "One.cs", "One.cs" },
            "Duplicate value");
    }

    [Fact]
    public void Load_WhenAllowedFilePatternsContainsDuplicates_Throws()
    {
        AssertInvalidShape(
            shape => shape.DirectoryRules[0].AllowedFilePatterns = new[] { "*.cs", "*.cs" },
            "Duplicate value");
    }

    private static void AssertInvalidShape(Action<ExchangeModuleLayoutShape> mutate, string expectedMessage)
    {
        var shape = CreateValidShape();
        mutate(shape);

        var repoRoot = CreateTempRepoWithShape(shape);
        try
        {
            var ex = Assert.Throws<InvalidDataException>(() => ExchangeModuleLayoutShape.Load(repoRoot));
            Assert.Contains(expectedMessage, ex.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(repoRoot, recursive: true);
        }
    }

    private static string CreateTempRepoWithShape(ExchangeModuleLayoutShape shape)
    {
        var repoRoot = Path.Combine(Path.GetTempPath(), "ExchangeApiShapeTests", Guid.NewGuid().ToString("N"));
        var layoutDirectory = Path.Combine(repoRoot, "docs", "normative", "layout");
        Directory.CreateDirectory(layoutDirectory);

        var shapePath = Path.Combine(layoutDirectory, "exchange-module-shape.json");
        File.WriteAllText(
            shapePath,
            JsonSerializer.Serialize(
                shape,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                }));

        return repoRoot;
    }

    private static ExchangeModuleLayoutShape CreateValidShape() =>
        new()
        {
            ExchangeRoot = new DirectoryRule
            {
                RequiredDirectories = new[] { "Wire" },
                AllowFiles = false,
            },
            DirectoryRules = new[]
            {
                new DirectoryRule
                {
                    Path = "Wire",
                    RequiredDirectories = new[] { "Public" },
                    AllowFiles = false,
                },
            },
            RequiredFiles = new[] { "Wire/ExchangeApi.Exchanges.{Exchange}.Wire.csproj" },
            AdapterRequiredFiles = new[] { "Adapter/Public/Api/PublicClient.cs" },
            AdapterMapPattern = "Adapter/Internal/Map/ContractMapper*.cs",
            ConditionalRules = new[]
            {
                new ConditionalRule
                {
                    Name = "PrivateSignerPlacement",
                    WhenDirectory = "Wire/Private/Endpoints",
                    RequiredDirectories = new[] { "Wire/Internal/Auth" },
                },
            },
        };
}
