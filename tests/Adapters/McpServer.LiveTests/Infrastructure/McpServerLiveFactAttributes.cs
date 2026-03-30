using ExchangeApi.Adapters.McpServer.Configuration;
using ExchangeApi.Tests.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Adapters.McpServer.LiveTests.Infrastructure;

internal sealed class McpServerPublicReadLiveFactAttribute : FactAttribute
{
    public McpServerPublicReadLiveFactAttribute()
    {
        Skip = McpServerLiveTestPolicy.GetPublicReadSkipReason();
    }
}

internal sealed class McpServerPrivateReadLiveFactAttribute : FactAttribute
{
    public McpServerPrivateReadLiveFactAttribute()
    {
        Skip = McpServerLiveTestPolicy.GetPrivateReadSkipReason();
    }
}

internal static class McpServerLiveTestPolicy
{
    public static string? GetPublicReadSkipReason()
    {
        return GlobalLiveOptInSkipReason("MCP server public live tests");
    }

    public static string? GetPrivateReadSkipReason()
    {
        var skip = GlobalLiveOptInSkipReason("MCP server private live tests");
        if (skip is not null)
        {
            return skip;
        }

        if (!BitflyerCredentialResolver.HasConfiguredCredentialsSource())
        {
            return $"Set {BitflyerCredentialResolver.CredentialsAgeFileEnvName} and {BitflyerCredentialResolver.AgeIdentityFileEnvName} to run MCP server private live tests.";
        }

        return null;
    }

    private static string? GlobalLiveOptInSkipReason(string target)
    {
        if (!LiveTestLocalPolicy.HasLiveOptIn())
        {
            return LiveTestLocalPolicy.LiveOptInMessage(target);
        }

        return null;
    }
}
