using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerRawCancelResponseTests
{
    [Fact]
    public async Task CancelChildOrder_allows_empty_success_body()
    {
        var api = new RawApi(new EmptySuccessWireExecutor());
        var request = new CancelChildOrderRequest
        {
            ProductCode = ProductCode.ParseOrThrowNormalized("BTC_JPY"),
            ChildOrderAcceptanceId = new FreeText("JRF-1"),
        };

        var call = await api.CancelChildOrderCallAsync(request);

        var ok = Assert.IsType<CallResult<ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos.CancelChildOrderResponse>.Ok>(call.Result);
        Assert.NotNull(ok.Response);
    }

    [Fact]
    public async Task CancelParentOrder_allows_empty_success_body()
    {
        var api = new RawApi(new EmptySuccessWireExecutor());
        var request = new CancelParentOrderRequest
        {
            ProductCode = ProductCode.ParseOrThrowNormalized("BTC_JPY"),
            ParentOrderAcceptanceId = new FreeText("JRF-1"),
        };

        var call = await api.CancelParentOrderCallAsync(request);

        var ok = Assert.IsType<CallResult<ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos.CancelParentOrderResponse>.Ok>(call.Result);
        Assert.NotNull(ok.Response);
    }

    [Fact]
    public async Task CancelAllChildOrders_allows_empty_success_body()
    {
        var api = new RawApi(new EmptySuccessWireExecutor());
        var request = new CancelAllChildOrdersRequest
        {
            ProductCode = ProductCode.ParseOrThrowNormalized("BTC_JPY"),
        };

        var call = await api.CancelAllChildOrdersCallAsync(request);

        var ok = Assert.IsType<CallResult<ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos.CancelAllChildOrdersResponse>.Ok>(call.Result);
        Assert.NotNull(ok.Response);
    }

    private sealed class EmptySuccessWireExecutor : IWireCallExecutor
    {
        public Task<Call<WireCallSpec, WireResponse>> SendAsync(
            WireCallSpec spec,
            CancellationToken cancellationToken = default)
        {
            var meta = CallMeta.CreateInternal("Wire", "EmptySuccessWireExecutor");
            var response = new WireResponse(StatusCode: 200, Json: string.Empty);
            var call = new Call<WireCallSpec, WireResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: spec,
                Result: new CallResult<WireResponse>.Ok(response),
                Meta: meta);
            return Task.FromResult(call);
        }
    }
}
