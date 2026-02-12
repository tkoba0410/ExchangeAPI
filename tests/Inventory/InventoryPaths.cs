using System.IO;

namespace ExchangeApi.Tests.Inventory;

internal static class InventoryPaths
{
    public const string BitflyerRelative = "docs/inventory/endpoints-bitflyer.md";
    public const string BittradeRelative = "docs/inventory/endpoints-bittrade.md";

    public static string BitflyerAbsolute() => ToAbsolute(BitflyerRelative);
    public static string BittradeAbsolute() => ToAbsolute(BittradeRelative);

    private static string ToAbsolute(string relativePath)
    {
        var repoRoot = InventoryEndpointIdParser.FindRepoRoot();
        return Path.Combine(repoRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }
}
