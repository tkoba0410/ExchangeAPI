using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record GetOrdersByOrderIdResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] GetOrdersByOrderIdDetail? Data);
