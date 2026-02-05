using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Wire.Internal;

public interface IBitflyerWireCallExecutor
{
    Task<Call<WireCallSpec, WireResponse>> SendAsync(
        WireCallSpec spec,
        CancellationToken cancellationToken = default);
}

public sealed class BitflyerWireCallExecutor : IBitflyerWireCallExecutor
{
    private readonly IWireTransport _wire;

    public BitflyerWireCallExecutor(IWireTransport wire)
    {
        _wire = wire ?? throw new ArgumentNullException(nameof(wire));
    }

    public Task<Call<WireCallSpec, WireResponse>> SendAsync(
        WireCallSpec spec,
        CancellationToken cancellationToken = default) =>
        _wire.SendAsync(spec, cancellationToken);
}
