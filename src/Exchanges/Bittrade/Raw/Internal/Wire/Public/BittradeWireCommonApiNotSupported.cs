using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireCommonApiNotSupported : IBittradeWireCommonApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire common is not supported.");

    public Task<RawTimestampResponse> GetTimestampAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken ct = default) =>
        throw NotSupported();

    public Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken ct = default) =>
        throw NotSupported();
}
