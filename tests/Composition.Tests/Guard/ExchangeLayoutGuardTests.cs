using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExchangeApi.Tests.Composition.Tests.Guard;

public sealed class ExchangeLayoutGuardTests
{
    [Fact]
    public void Exchanges_MustNotContain_Application_Directory()
    {
        var repositoryRoot = FindRepositoryRoot();
        var exchangesRoot = Path.Combine(repositoryRoot, "src", "Exchanges");
        var violations = Directory
            .EnumerateDirectories(exchangesRoot, "Application", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            violations.Length == 0,
            "Exchange layout violation(s):\n" + string.Join("\n", violations));
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var solutionPath = Path.Combine(current.FullName, "ExchangeApi.slnx");
            if (File.Exists(solutionPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repository root (ExchangeApi.slnx).");
    }
}
