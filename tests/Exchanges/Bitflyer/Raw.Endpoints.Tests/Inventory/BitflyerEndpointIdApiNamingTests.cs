using System.Collections.Generic;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
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
        EndpointIdNamingAssertions.AssertCallAsyncMethodsExist(inventory, typeof(INormalizedApi));
    }

    private static string InventoryFilePath => InventoryPaths.BitflyerAbsolute();

    private static IReadOnlyCollection<string> LoadInventoryEndpointIds()
    {
        return InventoryEndpointIdParser.ParseEndpointIdsFromFile(InventoryFilePath);
    }
}
