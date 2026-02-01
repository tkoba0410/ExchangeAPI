using System;
using System.IO;
using System.Linq;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Constants;
using ExchangeApi.Tests.Inventory;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Endpoints.Tests;

public sealed class BittradeInventoryEndpointIdConsistencyTests
{
    [Fact]
    public void Bittrade_InventoryEndpointIds_MustMatch_CodeEndpointIds()
    {
        var inventory = LoadInventoryEndpointIds();
        var code = BittradeEndpointIdCatalog.GetAllEndpointIds();

        var missing = inventory.Except(code, StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.True(missing.Length == 0, $"Missing EndpointIds: {string.Join(", ", missing)}");
    }

    [Fact]
    public void Bittrade_CodeEndpointIds_MustMatch_InventoryEndpointIds()
    {
        var inventory = LoadInventoryEndpointIds();
        var code = BittradeEndpointIdCatalog.GetAllEndpointIds();

        var extra = code.Except(inventory, StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.True(extra.Length == 0, $"Extra EndpointIds: {string.Join(", ", extra)}");
    }

    private static string InventoryFilePath =>
        Path.Combine(InventoryEndpointIdParser.FindRepoRoot(), "docs", "inventory", "endpoints-bittrade.md");

    private static System.Collections.Generic.HashSet<string> LoadInventoryEndpointIds() =>
        InventoryEndpointIdParser.ParseEndpointIdsFromFile(InventoryFilePath);
}
