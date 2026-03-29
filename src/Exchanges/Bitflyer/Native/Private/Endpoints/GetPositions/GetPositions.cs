using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;

public static class GetPositions
{
    public sealed class Item
    {
        [JsonPropertyName("product_code")]
        public required string ProductCode { get; init; }

        [JsonPropertyName("side")]
        public required BitflyerOrderSide Side { get; init; }

        [JsonPropertyName("price")]
        public required decimal Price { get; init; }

        [JsonPropertyName("size")]
        public required decimal Size { get; init; }

        [JsonPropertyName("commission")]
        public required decimal Commission { get; init; }

        [JsonPropertyName("swap_point_accumulate")]
        public required decimal SwapPointAccumulate { get; init; }

        [JsonPropertyName("require_collateral")]
        public required decimal RequireCollateral { get; init; }

        [JsonPropertyName("open_date")]
        public required DateTimeOffset OpenDate { get; init; }

        [JsonPropertyName("leverage")]
        public required decimal Leverage { get; init; }

        [JsonPropertyName("pnl")]
        public required decimal Pnl { get; init; }

        [JsonPropertyName("sfd")]
        public required decimal Sfd { get; init; }
    }
}
