using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireCommonApiNotSupported : IBittradeWireCommonApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire common is not supported.");

    public Task<WireCall> GetTimestampAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> GetSymbolsAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> GetCurrenciesAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> GetRetailMaintainTimeAsync(CancellationToken ct = default) =>
        throw NotSupported();
}
