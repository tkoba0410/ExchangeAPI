using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized;

internal sealed class BittradeNormalizedComponents
{
    public BittradeNormalizedPublicApi Public { get; }
    public BittradeNormalizedPrivateApi Private { get; }
    public FreeText AccountId { get; }

    public BittradeNormalizedComponents(
        BittradeNormalizedPublicApi publicApi,
        BittradeNormalizedPrivateApi privateApi,
        FreeText accountId)
    {
        Public = publicApi;
        Private = privateApi;
        AccountId = accountId;
    }
}
