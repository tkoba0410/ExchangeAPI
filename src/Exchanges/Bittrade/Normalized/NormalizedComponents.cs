using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal sealed class NormalizedComponents
{
    public NormalizedPublicApi Public { get; }
    public NormalizedPrivateApi Private { get; }
    public AccountId AccountId { get; }

    public NormalizedComponents(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi privateApi,
        AccountId accountId)
    {
        Public = publicApi;
        Private = privateApi;
        AccountId = accountId;
    }
}
