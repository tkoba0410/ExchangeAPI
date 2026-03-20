using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using NormalizedPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

namespace Exchange.Bitflyer.LiveTests;

public sealed class BitflyerWirePrivatePostLiveTests
{
    [BitflyerLivePostFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivatePost")]
    [Trait("Layer", "Wire")]
    public async Task SendChildOrder_ThenCancelChildOrder_CompletesLifecycle()
    {
        var order = BitflyerLiveSettings.GetPostOrder();
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var wire = new WireTransport(restClient);

        var sendCall = await wire.SendAsync(BitflyerLiveWireSpecs.SendChildOrder(order));
        var sendResponse = BitflyerLiveAssert.RequireWireSuccess(sendCall);
        var acceptanceId = BitflyerLiveAssert.ParseAcceptanceIdFromSendOrderJson(sendResponse.Json);

        try
        {
            await BitflyerLiveAssert.WaitForWireChildOrderVisibilityAsync(
                wire,
                order.ProductCode,
                acceptanceId,
                shouldExist: true);
        }
        finally
        {
            await BitflyerLiveAssert.CancelWireChildOrderAsync(wire, order, acceptanceId);
        }
    }
}

public sealed class BitflyerRawPrivatePostLiveTests
{
    [BitflyerLivePostFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivatePost")]
    [Trait("Layer", "Raw")]
    public async Task SendChildOrder_ThenCancelChildOrder_CompletesLifecycle()
    {
        var order = BitflyerLiveSettings.GetPostOrder();
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var sendCall = await raw.SendChildOrderCallAsync(BitflyerLiveAssert.CreateSendChildOrderRequest(order));
        var sendResponse = BitflyerLiveAssert.RequireOk(sendCall);
        var acceptanceId = sendResponse.ChildOrderAcceptanceId;
        Assert.False(string.IsNullOrWhiteSpace(acceptanceId));

        try
        {
            await BitflyerLiveAssert.WaitForRawChildOrderVisibilityAsync(
                raw,
                order.ProductCode,
                acceptanceId,
                shouldExist: true);
        }
        finally
        {
            await BitflyerLiveAssert.CancelRawChildOrderAsync(raw, order.ProductCode, acceptanceId);
        }
    }
}

public sealed class BitflyerNormalizedPrivatePostLiveTests
{
    [BitflyerLivePostFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivatePost")]
    [Trait("Layer", "Normalized")]
    public async Task SendChildOrder_ThenCancelChildOrder_CompletesLifecycle()
    {
        var order = BitflyerLiveSettings.GetPostOrder();
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();
        var request = new NormalizedPrivateRequests.OrderRequest(
            order.Symbol,
            order.Side,
            OrderType.Limit,
            order.Size,
            order.Price);

        var sendCall = await api.SendChildOrderCallAsync(request);
        var sendResponse = BitflyerLiveAssert.RequireOk(sendCall);
        Assert.Equal(OrderIdKind.AcceptanceId, sendResponse.Key.Kind);

        var acceptanceId = sendResponse.AcceptanceId?.Value ?? sendResponse.Key.Value;
        try
        {
            await BitflyerLiveAssert.WaitForNormalizedChildOrderVisibilityAsync(
                api,
                order.Symbol,
                acceptanceId,
                shouldExist: true);
        }
        finally
        {
            await BitflyerLiveAssert.CancelNormalizedChildOrderAsync(
                api,
                order.Symbol,
                sendResponse.Key,
                acceptanceId);
        }
    }
}
