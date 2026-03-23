using ExchangeApi.Stage10.Bitflyer.Native.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public sealed class BitflyerNativeClientBundle : IDisposable
{
    private readonly IDisposable _owner;
    private bool _disposed;

    internal BitflyerNativeClientBundle(
        IDisposable owner,
        BitflyerProtocolClientView protocol,
        IBitflyerPublicNativeApi publicApi,
        IBitflyerPrivateNativeApi? privateApi)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Protocol = protocol ?? throw new ArgumentNullException(nameof(protocol));
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
    }

    public BitflyerProtocolClientView Protocol { get; }

    public IBitflyerPublicNativeApi Public { get; }

    public IBitflyerPrivateNativeApi? Private { get; }

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
