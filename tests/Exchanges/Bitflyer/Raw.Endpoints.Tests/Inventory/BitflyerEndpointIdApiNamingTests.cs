using System.Collections.Generic;
using System.IO;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Tests.Inventory;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerEndpointIdApiNamingTests
{
    [Fact]
    public void Bitflyer_InventoryEndpointIds_MustHave_RawCallAsyncMethods()
    {
        var inventory = LoadInventoryEndpointIds();
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(IRawApi));
    }

    [Fact]
    public void Bitflyer_InventoryEndpointIds_MustHave_NormalizedCallAsyncMethods()
    {
        var inventory = LoadInventoryEndpointIds();
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(IBitflyerNormalizedApi));
    }

    private static string InventoryFilePath =>
        Path.Combine(InventoryEndpointIdParser.FindRepoRoot(), "docs", "inventory", "endpoints-bitflyer.md");

    private static IReadOnlyCollection<string> LoadInventoryEndpointIds()
    {
        var inventory = InventoryEndpointIdParser.ParseEndpointIdsFromFile(InventoryFilePath);
        var notImplemented = EndpointIdCatalog.GetNotImplementedEndpointIds();
        if (notImplemented.Count == 0)
        {
            return inventory;
        }

        inventory.ExceptWith(notImplemented);
        return inventory;
    }
}
