using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Internal;

public interface IBittradeWireCallExecutor
{
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        WireCallSpec spec,
        CancellationToken cancellationToken = default);
}

public sealed class WireCallExecutor : IBittradeWireCallExecutor
{
    private readonly IWireTransport _wire;

    public WireCallExecutor(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<WireCallSpec, WireResponse>> SendAsync(
        WireCallSpec spec,
        CancellationToken cancellationToken = default) =>
        _wire.SendAsync(spec, cancellationToken);
}
