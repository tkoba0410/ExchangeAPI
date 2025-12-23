using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Wire.Private;

internal sealed class BitflyerWireTradingApi : IBitflyerWireTradingApi
{
    private readonly IBitflyerRawTradingApi _raw;
    private readonly IBitflyerPrivateTradingApi _legacy;

    public BitflyerWireTradingApi(IBitflyerRawTradingApi raw, IBitflyerPrivateTradingApi legacy)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _legacy = legacy ?? throw new ArgumentNullException(nameof(legacy));
    }

    public async Task<CreateChildOrderResponse> CreateChildOrderAsync(
        CreateChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = MapSendChildOrderRequest(request);
        var rawResponse = await _raw.SendChildOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
        return MapSendChildOrderResponse(rawResponse);
    }

    public async Task<EmptyResponse> CancelChildOrderAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var rawRequest = MapCancelChildOrderRequest(request);
        await _raw.CancelChildOrderAsync(rawRequest, cancellationToken).ConfigureAwait(false);
        return new EmptyResponse();
    }

    public Task<EmptyResponse> CancelAllChildOrdersAsync(
        CancelAllChildOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CancelAllChildOrdersAsync(request, cancellationToken);

    public Task<CreateParentOrderResponse> CreateParentOrderAsync(
        CreateParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CreateParentOrderAsync(request, cancellationToken);

    public Task<EmptyResponse> CancelParentOrderAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CancelParentOrderAsync(request, cancellationToken);

    public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
        CreateWithdrawalRequest request,
        CancellationToken cancellationToken = default) =>
        _legacy.CreateWithdrawalAsync(request, cancellationToken);

    private static RawSendChildOrderRequest MapSendChildOrderRequest(CreateChildOrderRequest request) => new()
    {
        ProductCode = MapProductCode(request.ProductCode),
        ChildOrderType = MapChildOrderType(request.ChildOrderType),
        Side = MapSide(request.Side),
        Size = request.Size,
        Price = request.Price,
        MinuteToExpire = request.MinuteToExpire,
        TimeInForce = request.TimeInForce is null ? null : MapTimeInForce(request.TimeInForce.Value),
        TriggerPrice = request.TriggerPrice,
    };

    private static RawCancelChildOrderRequest MapCancelChildOrderRequest(CancelChildOrderRequest request) => new()
    {
        ProductCode = MapProductCode(request.ProductCode),
        ChildOrderId = request.ChildOrderId,
        ChildOrderAcceptanceId = request.ChildOrderAcceptanceId,
    };

    private static CreateChildOrderResponse MapSendChildOrderResponse(RawSendChildOrderResponse response) => new()
    {
        ChildOrderAcceptanceId = response.ChildOrderAcceptanceId,
    };

    private static string MapProductCode(ProductCode productCode) =>
        productCode switch
        {
            ProductCode.BtcJpy => "BTC_JPY",
            ProductCode.EthJpy => "ETH_JPY",
            ProductCode.FxBtcJpy => "FX_BTC_JPY",
            _ => throw new ArgumentOutOfRangeException(nameof(productCode), productCode, "Unsupported product_code."),
        };

    private static string MapChildOrderType(ChildOrderType childOrderType) =>
        childOrderType switch
        {
            ChildOrderType.Market => "MARKET",
            ChildOrderType.Limit => "LIMIT",
            _ => throw new ArgumentOutOfRangeException(nameof(childOrderType), childOrderType, "Unsupported child_order_type."),
        };

    private static string MapSide(Side side) =>
        side switch
        {
            Side.Buy => "BUY",
            Side.Sell => "SELL",
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, "Unsupported side."),
        };

    private static string MapTimeInForce(TimeInForce timeInForce) =>
        timeInForce switch
        {
            TimeInForce.Gtc => "GTC",
            TimeInForce.Ioc => "IOC",
            TimeInForce.Fok => "FOK",
            _ => throw new ArgumentOutOfRangeException(nameof(timeInForce), timeInForce, "Unsupported time_in_force."),
        };
}
