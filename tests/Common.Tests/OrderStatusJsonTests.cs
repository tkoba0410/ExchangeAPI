using System.Text.Json;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests;

public sealed class OrderStatusJsonTests
{
    [Fact]
    public void OrderStatus_RoundTrips_WithOrderKey()
    {
        var status = new OrderStatus(
            ProductCode: "BTC_JPY",
            Key: new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-1"),
            Status: OrderState.Completed,
            ExecutedSize: new Size(0.01m),
            OutstandingSize: new Size(0m),
            Price: new Price(100m),
            AveragePrice: new Price(100m));

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
