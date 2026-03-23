using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;

namespace ExchangeApi.Stage10.Bitflyer.Native.Private.Api;

public sealed class BitflyerPrivateNativeApi : IBitflyerPrivateNativeApi
{
    private readonly IGetBalanceNativeEndpoint _getBalance;
    private readonly ISendChildOrderNativeEndpoint _sendChildOrder;
    private readonly ICancelChildOrderNativeEndpoint _cancelChildOrder;

    public BitflyerPrivateNativeApi(
        IGetBalanceNativeEndpoint getBalance,
        ISendChildOrderNativeEndpoint sendChildOrder,
        ICancelChildOrderNativeEndpoint cancelChildOrder)
    {
        _getBalance = getBalance ?? throw new ArgumentNullException(nameof(getBalance));
        _sendChildOrder = sendChildOrder ?? throw new ArgumentNullException(nameof(sendChildOrder));
        _cancelChildOrder = cancelChildOrder ?? throw new ArgumentNullException(nameof(cancelChildOrder));
    }

    public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _getBalance.CallAsync(request, cancellationToken);

    public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _sendChildOrder.CallAsync(request, cancellationToken);

    public Task<Call<CancelChildOrderRequest, CancelChildOrderResponse>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _cancelChildOrder.CallAsync(request, cancellationToken);
}
