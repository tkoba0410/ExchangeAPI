using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal sealed class BittradeNormalizedAccountApi : IBittradeNormalizedAccountApi
{
    private readonly IBittradeRawApi _raw;
    private readonly string _accountId;

    public BittradeNormalizedAccountApi(IBittradeRawApi raw, string accountId)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<IReadOnlyList<BittradeBalanceEntryNormalized>> GetBalancesAsync(CancellationToken ct = default)
    {
        var response = await _raw.GetAccountBalanceAsync(_accountId, ct).ConfigureAwait(false);
        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Data is null)
        {
            throw new ExchangeApiException("Bittrade balance response invalid.");
        }

        return BittradeNormalizer.NormalizeBalances(response.Data);
    }
}
