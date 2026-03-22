using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public sealed class BitflyerWireClientBundle : IDisposable
{
    private readonly IDisposable _owner;
    private bool _disposed;

    internal BitflyerWireClientBundle(
        IDisposable owner,
        IBitflyerPublicWireApi publicApi,
        IBitflyerPrivateWireApi? privateApi)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
    }

    public IBitflyerPublicWireApi Public { get; }

    public IBitflyerPrivateWireApi? Private { get; }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _owner.Dispose();
    }
}
