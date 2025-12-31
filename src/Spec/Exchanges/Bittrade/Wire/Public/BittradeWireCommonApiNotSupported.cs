using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireCommonApiNotSupported : IBittradeWireCommonApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire common is not supported.");

    public Task<WireResponse<RawTimestampResponse>> GetTimestampAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse<RawSymbolsResponse>> GetSymbolsAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse<RawCurrenciesResponse>> GetCurrenciesAsync(CancellationToken ct = default) =>
        throw NotSupported();
}
