using System;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Normalize;

internal static class BittradeNormalizeFactory
{
    public static IRequestSigner CreateRequestSigner(string accessKey, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(accessKey))
        {
            throw new ArgumentException("accessKey is required.", nameof(accessKey));
        }

        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new ArgumentException("secretKey is required.", nameof(secretKey));
        }

        return new BittradeRequestSigner(accessKey, secretKey);
    }

    public static BittradeNormalizeBundle FromRestClient(IRestClient restClient, string? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var normalizedAccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
        var raw = new BittradeRawApi(restClient);

        var marketData = new BittradeNormalizedMarketDataApi(raw.MarketData);
        var exchangeInfo = new BittradeNormalizedExchangeInfoApi(raw);
        var account = normalizedAccountId is null
            ? null
            : new BittradeNormalizedAccountApi(raw, normalizedAccountId);

        return new BittradeNormalizeBundle(
            marketData: marketData,
            exchangeInfo: exchangeInfo,
            account: account,
            rawBundle: raw,
            accountId: normalizedAccountId);
    }
}
