using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Extensions;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Core.Contracts.Errors;

namespace ExchangeApi.Common.Tests;

public sealed class ExchangeClientExtensionsTests
{
    private sealed class DummyClient : IExchangeClient
    {
        public ExchangeCode ExchangeCode => ExchangeCode.Bitflyer;
    }

    [Fact]
    public void Raw_Throws_When_NotSupported()
    {
        var client = new DummyClient();
        Assert.Throws<ExchangeFeatureNotSupportedException>(() => client.Raw<object>());
    }

    [Fact]
    public void Wire_Throws_When_NotSupported()
    {
        var client = new DummyClient();
        Assert.Throws<ExchangeFeatureNotSupportedException>(() => client.Wire<object>());
    }
}
