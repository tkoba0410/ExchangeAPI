namespace ExchangeApi.Exchanges.Bitflyer.Composition.Internal.Runtime;

internal sealed class SharedBundleLifetime
{
    private readonly IDisposable? _ownedResource;
    private int _leaseCount;
    private int _resourceDisposed;

    private SharedBundleLifetime(IDisposable? ownedResource)
    {
        _ownedResource = ownedResource;
    }

    internal static SharedBundleLifetime CreateOwned(IDisposable ownedResource)
    {
        ArgumentNullException.ThrowIfNull(ownedResource);

        return new SharedBundleLifetime(ownedResource);
    }

    internal static SharedBundleLifetime CreateExternal()
    {
        return new SharedBundleLifetime(null);
    }

    internal IDisposable AcquireLease()
    {
        Interlocked.Increment(ref _leaseCount);
        return new Lease(this);
    }

    private void ReleaseLease()
    {
        if (Interlocked.Decrement(ref _leaseCount) == 0 && Interlocked.Exchange(ref _resourceDisposed, 1) == 0)
        {
            _ownedResource?.Dispose();
        }
    }

    private sealed class Lease : IDisposable
    {
        private SharedBundleLifetime? _owner;

        internal Lease(SharedBundleLifetime owner)
        {
            _owner = owner;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _owner, null)?.ReleaseLease();
        }
    }
}
