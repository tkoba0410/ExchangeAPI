using System.Collections.Generic;
using System.IO;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Tests.Inventory;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Endpoints.Tests;

public sealed class EndpointIdApiNamingTests
{
    [Fact]
    public void Bittrade_InventoryEndpointIds_MustHave_RawCallAsyncMethods()
    {
        var inventory = LoadInventoryEndpointIds();
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(IBittradeRawApi));
    }

    [Fact]
    public void Bittrade_InventoryEndpointIds_MustHave_NormalizedCallAsyncMethods()
    {
        var inventory = LoadInventoryEndpointIds();
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(IBittradeNormalizedApi));
    }

    private static string InventoryFilePath =>
        Path.Combine(InventoryEndpointIdParser.FindRepoRoot(), "docs", "inventory", "endpoints-bittrade.md");

    private static IReadOnlyCollection<string> LoadInventoryEndpointIds() =>
        InventoryEndpointIdParser.ParseEndpointIdsFromFile(InventoryFilePath);
}
