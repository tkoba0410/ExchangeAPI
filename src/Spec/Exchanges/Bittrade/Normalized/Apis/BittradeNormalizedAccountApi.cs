using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
using ExchangeApi.Spec.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal sealed class BittradeNormalizedAccountApi : IBittradeNormalizedAccountApi
{
    private readonly IBittradeRawApi _raw;
    private readonly string _accountId;

    internal BittradeNormalizedAccountApi(IBittradeRawApi raw, string accountId)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<IReadOnlyList<BittradeBalanceEntryNormalized>> GetBalancesAsync(CancellationToken ct = default)
    {
        var response = await _raw.GetAccountBalanceAsync(_accountId, ct).ConfigureAwait(false);
        if (!string.Equals(response.Status, "ok", StringComparison.OrdinalIgnoreCase) || response.Data is null)
        {
            throw new BittradeNormalizedException("Bittrade balance response invalid.");
        }

        return BittradeNormalizer.NormalizeBalances(response.Data);
    }

    public async Task<BittradeNormalizedCall<IReadOnlyList<BittradeBalanceEntryNormalized>, JsonElement>> GetBalancesCallAsync(
        CancellationToken ct = default)
    {
        var rawCall = await _raw.GetAccountBalanceCallAsync(_accountId, ct).ConfigureAwait(false);
        var request = CreateRequest("Bittrade.GetAccountBalance", new Dictionary<string, string?>
        {
            ["accountId"] = _accountId,
        });

        return CreateCall(
            rawCall,
            request,
            ok =>
            {
                if (!string.Equals(ok.Status, "ok", StringComparison.OrdinalIgnoreCase) || ok.Data is null)
                {
                    throw new BittradeNormalizedException("Bittrade balance response invalid.");
                }

                return BittradeNormalizer.NormalizeBalances(ok.Data);
            });
    }

    private static BittradeNormalizedRequest CreateRequest(
        string operation,
        IReadOnlyDictionary<string, string?> parameters) =>
        new(operation, parameters);

    private static BittradeNormalizedCall<TOk, JsonElement> CreateCall<TRaw, TOk>(
        BittradeRawCall<TRaw, JsonElement> rawCall,
        BittradeNormalizedRequest request,
        Func<TRaw, TOk> mapper)
    {
        return rawCall.Result switch
        {
            Ok<TRaw, JsonElement> ok => new BittradeNormalizedCall<TOk, JsonElement>(
                request,
                new Ok<TOk, JsonElement>(mapper(ok.Value), ok.StatusCode),
                rawCall.Meta),
            Err<TRaw, JsonElement> err => new BittradeNormalizedCall<TOk, JsonElement>(
                request,
                new Err<TOk, JsonElement>(err.Error, err.StatusCode),
                rawCall.Meta),
            _ => throw new InvalidOperationException("Unsupported CallResult type.")
        };
    }
}
