using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer;
using ExchangeApi.Adapter.Bitflyer.Models;

namespace ExchangeApi.Adapter.Bitflyer.Tests.Fakes;

public sealed class FakeBitflyerPrivateTradingApi : IBitflyerPrivateTradingApi
{
    private readonly BitflyerSendChildOrderResponse _response;
    private readonly Exception? _exceptionToThrow;

    public BitflyerSendChildOrderRequest? LastRequest { get; private set; }
    public BitflyerCancelChildOrderRequest? LastCancelRequest { get; private set; }
    public BitflyerCancelAllChildOrdersRequest? LastCancelAllRequest { get; private set; }
    public Dictionary<string, object?>? LastParentOrderBody { get; private set; }
    public Dictionary<string, object?>? LastWithdrawBody { get; private set; }

    public FakeBitflyerPrivateTradingApi(
        BitflyerSendChildOrderResponse response,
        Exception? exceptionToThrow = null)
    {
        _response = response;
        _exceptionToThrow = exceptionToThrow;
    }

    public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (_exceptionToThrow is not null)
        {
            throw _exceptionToThrow;
        }

        LastRequest = request;
        return Task.FromResult(_response);
    }

    public Task<BitflyerEmptyResponse> CancelChildOrderAsync(BitflyerCancelChildOrderRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelRequest = request;
        return Task.FromResult(new BitflyerEmptyResponse());
    }

    public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(BitflyerCancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)
    {
        LastCancelAllRequest = request;
        return Task.FromResult(new BitflyerEmptyResponse());
    }

    public Task<JsonElement> SendParentOrderAsync(Dictionary<string, object?> body, CancellationToken cancellationToken = default)
    {
        LastParentOrderBody = body;
        return Task.FromResult(JsonDocument.Parse("{}").RootElement);
    }

    public Task<BitflyerEmptyResponse> CancelParentOrderAsync(Dictionary<string, object?> body, CancellationToken cancellationToken = default)
    {
        LastParentOrderBody = body;
        return Task.FromResult(new BitflyerEmptyResponse());
    }

    public Task<JsonElement> WithdrawAsync(Dictionary<string, object?> body, CancellationToken cancellationToken = default)
    {
        LastWithdrawBody = body;
        return Task.FromResult(JsonDocument.Parse("{}").RootElement);
    }
}
