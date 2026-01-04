using System;
using System.Collections.Generic;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerParentOrderMapperTests
{
    [Fact]
    public void Map_MapsParentOrderFields()
    {
        var symbol = new Symbol("BTC/JPY");
        var normalized = new BitflyerParentOrderNormalized(
            Id: 1,
            ParentOrderId: "PO-1",
            ProductCode: "BTC_JPY",
            Side: BitflyerSide.Buy,
            ParentOrderType: BitflyerParentOrderType.Ifd,
            Price: 123.45m,
            AveragePrice: 120.00m,
            Size: 0.5m,
            ParentOrderState: BitflyerParentOrderState.Active,
            ExpireDate: DateTimeOffset.UnixEpoch.AddHours(1),
            ParentOrderDate: DateTimeOffset.UnixEpoch,
            ParentOrderAcceptanceId: "PA-1",
            OutstandingSize: 0.2m,
            CancelSize: 0.1m,
            ExecutedSize: 0.3m,
            TotalCommission: 0.01m);

        var mapped = BitflyerParentOrderMapper.Map(symbol, normalized);

        Assert.Equal(ExchangeCode.Bitflyer, mapped.Exchange);
        Assert.Equal(symbol, mapped.Symbol);
        Assert.Equal("PO-1", mapped.ParentOrderId);
        Assert.Equal("PA-1", mapped.ParentOrderAcceptanceId);
        Assert.Equal(Side.Buy, mapped.Side);
        Assert.Equal("IFD", mapped.ParentOrderType);
        Assert.Equal("ACTIVE", mapped.ParentOrderState);
        Assert.Equal(123.45m, mapped.Price?.Value);
        Assert.Equal(120.00m, mapped.AveragePrice?.Value);
        Assert.Equal(0.5m, mapped.Size.Value);
        Assert.Equal(0.2m, mapped.OutstandingSize.Value);
        Assert.Equal(0.1m, mapped.CancelSize.Value);
        Assert.Equal(0.3m, mapped.ExecutedSize.Value);
        Assert.Equal(0.01m, mapped.TotalCommission);
        Assert.Equal(DateTimeOffset.UnixEpoch, mapped.ParentOrderDate);
        Assert.Equal(DateTimeOffset.UnixEpoch.AddHours(1), mapped.ExpireDate);
    }

    [Fact]
    public void MapDetail_MapsParametersAndDefaults()
    {
        var parameters = new List<BitflyerParentOrderParameterNormalized>
        {
            new(
                ProductCode: "BTC_JPY",
                ConditionType: BitflyerConditionType.StopLimit,
                Side: BitflyerSide.Sell,
                Size: 1.2m,
                Price: 0m,
                TriggerPrice: 345.67m,
                Offset: 0.5m)
        };

        var normalized = new BitflyerParentOrderDetailNormalized(
            Id: 2,
            ParentOrderId: "PO-2",
            OrderMethod: BitflyerOrderMethod.IfdOco,
            ExpireDate: DateTimeOffset.UnixEpoch.AddDays(1),
            TimeInForce: BitflyerTimeInForce.Gtc,
            Parameters: parameters,
            ParentOrderAcceptanceId: "PA-2");

        var mapped = BitflyerParentOrderMapper.MapDetail(normalized);

        Assert.Equal(ExchangeCode.Bitflyer, mapped.Exchange);
        Assert.Equal("PO-2", mapped.ParentOrderId);
        Assert.Equal("PA-2", mapped.ParentOrderAcceptanceId);
        Assert.Equal("IFDOCO", mapped.OrderMethod);
        Assert.Equal("GTC", mapped.TimeInForce);
        Assert.Single(mapped.Parameters);

        var parameter = mapped.Parameters[0];
        Assert.Equal("BTC_JPY", parameter.ProductCode);
        Assert.Equal("STOP_LIMIT", parameter.ConditionType);
        Assert.Equal(Side.Sell, parameter.Side);
        Assert.Equal(1.2m, parameter.Size.Value);
        Assert.Null(parameter.Price);
        Assert.Equal(345.67m, parameter.TriggerPrice?.Value);
        Assert.Equal(0.5m, parameter.Offset);
    }
}
