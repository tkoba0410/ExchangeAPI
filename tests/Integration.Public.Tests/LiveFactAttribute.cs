using System;
using Xunit;

namespace Integration.Public.Tests;

/// <summary>
/// LIVE_PUBLIC が設定されている場合にのみ実行される Fact。
/// デフォルトではスキップされ、実働 API へのアクセスを防ぐ。
/// </summary>
public sealed class LiveFactAttribute : FactAttribute
{
    private const string DefaultEnvVar = "LIVE_PUBLIC";

    public LiveFactAttribute(string envVar = DefaultEnvVar)
    {
        var enabled = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(enabled))
        {
            Skip = $"Set {envVar}=1 to run live public API tests.";
        }
    }
}
