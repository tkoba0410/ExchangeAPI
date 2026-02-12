using System.Collections.Generic;
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
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(IRawApi));
    }

    [Fact]
    public void Bittrade_InventoryEndpointIds_MustHave_NormalizedCallAsyncMethods()
    {
        var inventory = LoadInventoryEndpointIds();
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(INormalizedApi));
    }

    private static string InventoryFilePath => InventoryPaths.BittradeAbsolute();

    private static IReadOnlyCollection<string> LoadInventoryEndpointIds() =>
        InventoryEndpointIdParser.ParseEndpointIdsFromFile(InventoryFilePath);
}
