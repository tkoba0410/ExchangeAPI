using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Public;

internal sealed class BittradeWireCommonApi : IBittradeWireCommonApi
{
    private readonly BittradeRawApi _raw;

    public BittradeWireCommonApi(BittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public Task<RawTimestampResponse> GetTimestampAsync(CancellationToken ct = default) =>
        _raw.GetTimestampAsync(ct);

    public Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken ct = default) =>
        _raw.GetSymbolsAsync(ct);

    public Task<RawCurrenciesResponse> GetCurrenciesAsync(CancellationToken ct = default) =>
        _raw.GetCurrenciesAsync(ct);
}
