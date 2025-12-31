using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireCommonApiNotSupported : IBittradeWireCommonApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire common is not supported.");

    public Task<WireResponse> GetTimestampAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> GetSymbolsAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> GetCurrenciesAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> GetRetailMaintainTimeAsync(CancellationToken ct = default) =>
        throw NotSupported();
}
