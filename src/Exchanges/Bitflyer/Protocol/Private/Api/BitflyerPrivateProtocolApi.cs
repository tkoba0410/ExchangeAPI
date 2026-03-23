using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Api;

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
        _getBalance = getBalance;
        _sendChildOrder = sendChildOrder;
        _cancelChildOrder = cancelChildOrder;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getBalance.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _sendChildOrder.SendAsync(bodyJson, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> CancelChildOrderCallAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return _cancelChildOrder.SendAsync(bodyJson, cancellationToken);
    }
}
