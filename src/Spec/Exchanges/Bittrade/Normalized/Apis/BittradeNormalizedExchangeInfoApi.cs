using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal sealed class BittradeNormalizedExchangeInfoApi : IBittradeNormalizedExchangeInfoApi
{
    private readonly IBittradeRawApi _raw;

    internal BittradeNormalizedExchangeInfoApi(IBittradeRawApi raw)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
    }

    public async Task<IReadOnlyList<BittradeSymbolNormalized>> GetSymbolsAsync(CancellationToken ct = default)
    {
        var response = await _raw.GetSymbolsAsync(ct).ConfigureAwait(false);
        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Data is null)
        {
            throw new BittradeNormalizedException("Bittrade symbols response invalid.");
        }

        return BittradeNormalizer.NormalizeSymbols(response.Data);
    }
}
