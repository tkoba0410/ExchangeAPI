using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

/// <summary>
/// ファンディングレート (/v1/getfundingrate) のレスポンス DTO。
/// </summary>
internal sealed record FundingRateResponse(
    [property: JsonPropertyName("current_funding_rate")] decimal CurrentFundingRate,
    [property: JsonPropertyName("next_funding_rate_settledate")] DateTimeOffset NextFundingRateSettleDate);
