using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Normalized.Public.Api;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Factory;

public sealed class BitflyerNormalizedClientBundle : IDisposable
{
    private readonly IDisposable _owner;
    private bool _disposed;

    internal BitflyerNormalizedClientBundle(
        IDisposable owner,
        BitflyerWireClientView wire,
        IBitflyerPublicNormalizedApi publicApi,
        IBitflyerPrivateNormalizedApi? privateApi)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        Wire = wire ?? throw new ArgumentNullException(nameof(wire));
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
    }

    public BitflyerWireClientView Wire { get; }

    public IBitflyerPublicNormalizedApi Public { get; }

    public IBitflyerPrivateNormalizedApi? Private { get; }

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
