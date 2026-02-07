using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal sealed class NormalizedComponents
{
    public NormalizedPublicApi Public { get; }
    public NormalizedPrivateApi Private { get; }
    public FreeText AccountId { get; }

    public NormalizedComponents(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi privateApi,
        FreeText accountId)
    {
        Public = publicApi;
        Private = privateApi;
        AccountId = accountId;
    }
}
