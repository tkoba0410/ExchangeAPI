using System;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerParentOrderNormalizerTests
{
    [Fact]
    public void Normalize_PopulatesRawSnapshot()
    {
        var raw = CreateRawParentOrder("PO-1", "ACTIVE");
        var rawJson = "{\"parent_order_id\":\"PO-1\",\"parent_order_state\":\"ACTIVE\"}";

        var normalized = BitflyerParentOrderNormalizer.Normalize(raw, rawJson);

        Assert.Equal("PO-1", normalized.RawSnapshot.GetProperty("parent_order_id").GetString());
    }

    [Fact]
    public void Normalize_PreservesUnknownParentOrderState()
    {
        var raw = CreateRawParentOrder("PO-2", "NEW_STATE");

        var normalized = BitflyerParentOrderNormalizer.Normalize(raw, "{}");

        Assert.False(normalized.ParentOrderState.IsKnown);
        Assert.Equal("NEW_STATE", normalized.ParentOrderState.Unknown);
    }

    private static ParentOrderResponse CreateRawParentOrder(string parentOrderId, string parentOrderState)
    {
        return new ParentOrderResponse
        {
            Id = 1,
            ParentOrderId = parentOrderId,
            ProductCode = "BTC_JPY",
            Side = "BUY",
            ParentOrderType = "IFD",
            Price = 123.45m,
            AveragePrice = 120.00m,
            Size = 0.5m,
            ParentOrderState = parentOrderState,
            ExpireDate = DateTimeOffset.UnixEpoch.AddHours(1),
            ParentOrderDate = DateTimeOffset.UnixEpoch,
            ParentOrderAcceptanceId = "PA-1",
            OutstandingSize = 0.2m,
            CancelSize = 0.1m,
            ExecutedSize = 0.3m,
            TotalCommission = 0.01m
        };
    }
}
