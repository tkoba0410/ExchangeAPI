using System.Text.Json;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using Xunit;

namespace Common.Tests;

public sealed class OrderStatusJsonTests
{
    [Fact]
    public void OrderStatus_RoundTrips_WithOrderKey()
    {
        var status = new OrderStatus(
            ProductCode: "BTC_JPY",
            Key: new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-1"),
            Status: OrderState.Completed,
            ExecutedSize: 0.01m,
            OutstandingSize: 0m,
            Price: 100m,
            AveragePrice: 100m);

        var json = JsonSerializer.Serialize(status);
        var restored = JsonSerializer.Deserialize<OrderStatus>(json);

        Assert.NotNull(restored);
        Assert.Equal(status.ProductCode, restored!.ProductCode);
        Assert.Equal(status.Key.Kind, restored.Key.Kind);
        Assert.Equal(status.Key.Value, restored.Key.Value);
        Assert.Equal(status.Status, restored.Status);
        Assert.Equal(status.ExecutedSize, restored.ExecutedSize);
        Assert.Equal(status.OutstandingSize, restored.OutstandingSize);
        Assert.Equal(status.Price, restored.Price);
        Assert.Equal(status.AveragePrice, restored.AveragePrice);
    }
}
