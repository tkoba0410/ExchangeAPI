using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal.Encoding;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerParentOrderNormalizedTests
{
    [Fact]
    public void NormalizeList_keeps_state_and_raw_snapshot()
    {
        var json = """
        [
          {
            "id": 138398,
            "parent_order_id": "JCO20150707-084555-022523",
            "product_code": "BTC_JPY",
            "side": "BUY",
            "parent_order_type": "STOP",
            "price": 30000,
            "average_price": 30000,
            "size": 0.1,
            "parent_order_state": "COMPLETED",
            "expire_date": "2015-07-14T07:25:52",
            "parent_order_date": "2015-07-07T08:45:53",
            "parent_order_acceptance_id": "JRF20150707-084552-031927",
            "outstanding_size": 0,
            "cancel_size": 0,
            "executed_size": 0.1,
            "total_commission": 0
          }
        ]
        """;

        var raw = BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse>>(
            json,
            "Bitflyer.GetParentOrders");
        var normalized = BitflyerParentOrderNormalizer.NormalizeList(raw, json);

        Assert.Single(normalized);
        Assert.True(normalized[0].ParentOrderState.IsKnown);
        Assert.Equal(BitflyerParentOrderState.Completed, normalized[0].ParentOrderState.Known);
        Assert.Equal(JsonValueKind.Object, normalized[0].RawSnapshot.ValueKind);
    }

    [Fact]
    public void NormalizeDetail_keeps_order_method_and_raw_snapshot()
    {
        var json = """
        {
          "id": 4242,
          "parent_order_id": "JCP20150825-046876-036161",
          "order_method": "IFDOCO",
          "expire_date": "2015-09-24T04:35:59.277",
          "time_in_force": "GTC",
          "parameters": [{
            "product_code": "BTC_JPY",
            "condition_type": "LIMIT",
            "side": "BUY",
            "price": 30000,
            "size": 0.1,
            "trigger_price": 0,
            "offset": 0
          }],
          "parent_order_acceptance_id": "JRF20150925-060559-396699"
        }
        """;

        var raw = BitflyerRawJson.DeserializeOrThrow<RawPrivateDtos.RawGetParentOrderResponse>(
            json,
            "Bitflyer.GetParentOrder");
        var normalized = BitflyerParentOrderNormalizer.NormalizeDetail(raw, json);

        Assert.True(normalized.OrderMethod.IsKnown);
        Assert.Equal(BitflyerOrderMethod.IfdOco, normalized.OrderMethod.Known);
        Assert.Single(normalized.Parameters);
        Assert.Equal(JsonValueKind.Object, normalized.RawSnapshot.ValueKind);
    }
}
