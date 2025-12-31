using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;

internal sealed class BittradeWireCommonApi : IBittradeWireCommonApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly BittradeRawApi _raw;

    public BittradeWireCommonApi(BittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<WireResponse<RawTimestampResponse>> GetTimestampAsync(CancellationToken ct = default)
    {
        var raw = await _raw.GetTimestampAsync(ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, BittradeWireOperations.Common.GetTimestamp);
        return new WireResponse<RawTimestampResponse>(Exchange, raw);
    }

    public async Task<WireResponse<RawSymbolsResponse>> GetSymbolsAsync(CancellationToken ct = default)
    {
        var raw = await _raw.GetSymbolsAsync(ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, BittradeWireOperations.Common.GetSymbols);
        return new WireResponse<RawSymbolsResponse>(Exchange, raw);
    }

    public async Task<WireResponse<RawCurrenciesResponse>> GetCurrenciesAsync(CancellationToken ct = default)
    {
        var raw = await _raw.GetCurrenciesAsync(ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, BittradeWireOperations.Common.GetCurrencies);
        return new WireResponse<RawCurrenciesResponse>(Exchange, raw);
    }
}
