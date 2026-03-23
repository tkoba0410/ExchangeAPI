using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Units;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;

public interface IBitflyerPrivateNativeApi
{
    Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<SendChildOrderRequest, SendChildOrderResponse>> SendChildOrderCallAsync(
        SendChildOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelChildOrderRequest, Unit>> CancelChildOrderCallAsync(
        CancelChildOrderRequest request,
        CancellationToken cancellationToken = default);
}
