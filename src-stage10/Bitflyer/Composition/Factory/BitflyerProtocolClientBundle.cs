using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public sealed class BitflyerProtocolClientBundle : IDisposable
{
    private readonly IDisposable _owner;
    private bool _disposed;

    internal BitflyerProtocolClientBundle(
        IDisposable owner,
        IBitflyerPublicProtocolApi publicApi,
        IBitflyerPrivateProtocolApi? privateApi)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
    }

    public IBitflyerPublicProtocolApi Public { get; }

    public IBitflyerPrivateProtocolApi? Private { get; }

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
