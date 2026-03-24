using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralAccounts;

public static class GetCollateralAccounts
{
    public sealed class Item
    {
        [JsonPropertyName("currency_code")]
        public required string CurrencyCode { get; init; }

        [JsonPropertyName("amount")]
        public required decimal Amount { get; init; }
    }
}
