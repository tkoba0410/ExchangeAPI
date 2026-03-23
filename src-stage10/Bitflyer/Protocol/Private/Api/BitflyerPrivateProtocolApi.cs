using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;

public sealed class BitflyerPrivateProtocolApi : IBitflyerPrivateProtocolApi
{
    private readonly IGetBalanceProtocolEndpoint _getBalance;
    private readonly ISendChildOrderProtocolEndpoint _sendChildOrder;
    private readonly ICancelChildOrderProtocolEndpoint _cancelChildOrder;

    public BitflyerPrivateProtocolApi(
        IGetBalanceProtocolEndpoint getBalance,
        ISendChildOrderProtocolEndpoint sendChildOrder,
        ICancelChildOrderProtocolEndpoint cancelChildOrder)
    {
        _getBalance = getBalance ?? throw new ArgumentNullException(nameof(getBalance));
        _sendChildOrder = sendChildOrder ?? throw new ArgumentNullException(nameof(sendChildOrder));
        _cancelChildOrder = cancelChildOrder ?? throw new ArgumentNullException(nameof(cancelChildOrder));
    }

    public Task<Call<WireCallSpec, WireResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _getBalance.SendAsync(cancellationToken);

    public Task<Call<WireCallSpec, WireResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _sendChildOrder.SendAsync(bodyJson, cancellationToken);

    public Task<Call<WireCallSpec, WireResponse>> CancelChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default) =>
        _cancelChildOrder.SendAsync(bodyJson, cancellationToken);
}
