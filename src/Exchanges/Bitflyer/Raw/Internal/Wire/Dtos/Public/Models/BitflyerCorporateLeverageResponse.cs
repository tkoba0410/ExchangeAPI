using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

/// <summary>
/// 法人アカウント最大レバレッジ (/v1/getcorporateleverage) のレスポンス DTO。
/// </summary>
internal sealed record CorporateLeverageResponse(
    [property: JsonPropertyName("current_max")] decimal CurrentMax,
    [property: JsonPropertyName("current_startdate")] DateTimeOffset CurrentStartDate,
    [property: JsonPropertyName("next_max")] decimal? NextMax,
    [property: JsonPropertyName("next_startdate")] DateTimeOffset? NextStartDate);
