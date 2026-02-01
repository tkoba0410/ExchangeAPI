using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized;

internal sealed class BittradeNormalizedComponents
{
    public BittradeNormalizedPublicApi Public { get; }
    public BittradeNormalizedPrivateApi Private { get; }
    public string? AccountId { get; }

    public BittradeNormalizedComponents(
        BittradeNormalizedPublicApi publicApi,
        BittradeNormalizedPrivateApi privateApi,
        string? accountId)
    {
        Public = publicApi;
        Private = privateApi;
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }
}
