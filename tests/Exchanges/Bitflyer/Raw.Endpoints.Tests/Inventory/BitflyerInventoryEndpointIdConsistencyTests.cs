using System;
using System.IO;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Constants;
using ExchangeApi.Tests.Inventory;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerInventoryEndpointIdConsistencyTests
{
    [Fact]
    public void Bitflyer_InventoryEndpointIds_MustMatch_CodeEndpointIds()
    {
        var inventory = LoadInventoryEndpointIds();
        var code = BitflyerEndpointIdCatalog.GetAllEndpointIds();

        var missing = inventory.Except(code, StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.True(missing.Length == 0, $"Missing EndpointIds: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Bitflyer_CodeEndpointIds_MustMatch_InventoryEndpointIds()
    {
        var inventory = LoadInventoryEndpointIds();
        var code = BitflyerEndpointIdCatalog.GetAllEndpointIds();

        var extra = code.Except(inventory, StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.True(extra.Length == 0, $"Extra EndpointIds: {string.Join(", ", extra)}");
    }

    private static string InventoryFilePath =>
        Path.Combine(InventoryEndpointIdParser.FindRepoRoot(), "docs", "inventory", "endpoints-bitflyer.md");

    private static System.Collections.Generic.HashSet<string> LoadInventoryEndpointIds() =>
        InventoryEndpointIdParser.ParseEndpointIdsFromFile(InventoryFilePath);
}
