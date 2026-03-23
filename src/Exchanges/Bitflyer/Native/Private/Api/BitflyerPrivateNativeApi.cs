using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;

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
        _getBalance = getBalance;
        _sendChildOrder = sendChildOrder;
        _cancelChildOrder = cancelChildOrder;
    }

    public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBalance.CallAsync(request, cancellationToken);
    }

    public Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _sendChildOrder.CallAsync(request, cancellationToken);
    }

    public Task<Call<CancelChildOrderRequest, Unit>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        return _cancelChildOrder.CallAsync(request, cancellationToken);
    }
}
