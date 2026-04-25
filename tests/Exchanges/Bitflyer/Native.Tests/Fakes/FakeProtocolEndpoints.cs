using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.Withdraw;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

internal sealed class FakeGetMarketsProtocolEndpoint : IGetMarketsProtocolEndpoint
{
    private readonly Func<CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetMarketsProtocolEndpoint(Func<CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler());
    }
}

internal sealed class FakeGetBoardProtocolEndpoint : IGetBoardProtocolEndpoint
{
    private readonly Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetBoardProtocolEndpoint(Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetBoardStateProtocolEndpoint : IGetBoardStateProtocolEndpoint
{
    private readonly Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetBoardStateProtocolEndpoint(Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetHealthProtocolEndpoint : IGetHealthProtocolEndpoint
{
    private readonly Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetHealthProtocolEndpoint(Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetFundingRateProtocolEndpoint : IGetFundingRateProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetFundingRateProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetCorporateLeverageProtocolEndpoint : IGetCorporateLeverageProtocolEndpoint
{
    private readonly Func<CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetCorporateLeverageProtocolEndpoint(Func<CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler());
    }
}

internal sealed class FakeGetChatsProtocolEndpoint : IGetChatsProtocolEndpoint
{
    private readonly Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetChatsProtocolEndpoint(Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? fromDate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(fromDate));
    }
}

internal sealed class FakeGetExecutionsPublicProtocolEndpoint : IGetExecutionsPublicProtocolEndpoint
{
    private readonly Func<string?, int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetExecutionsPublicProtocolEndpoint(Func<string?, int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode, count, before, after));
    }
}

internal sealed class FakeGetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
{
    private readonly Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetTickerProtocolEndpoint(Func<string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
{
    private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetBalanceProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public IApiCredentialSession? LastCredentialSession { get; private set; }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        LastCredentialSession = null;
        return Task.FromResult(_call);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        LastCredentialSession = credentialSession;
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetPermissionsProtocolEndpoint : IGetPermissionsProtocolEndpoint
{
    private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetPermissionsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetCollateralProtocolEndpoint : IGetCollateralProtocolEndpoint
{
    private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetCollateralProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetCollateralAccountsProtocolEndpoint : IGetCollateralAccountsProtocolEndpoint
{
    private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetCollateralAccountsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetAddressesProtocolEndpoint : IGetAddressesProtocolEndpoint
{
    private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetAddressesProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetCoinInsProtocolEndpoint : IGetCoinInsProtocolEndpoint
{
    private readonly Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetCoinInsProtocolEndpoint(Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(count, before, after));
    }
}

internal sealed class FakeGetCoinOutsProtocolEndpoint : IGetCoinOutsProtocolEndpoint
{
    private readonly Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetCoinOutsProtocolEndpoint(Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(count, before, after));
    }
}

internal sealed class FakeGetBankAccountsProtocolEndpoint : IGetBankAccountsProtocolEndpoint
{
    private readonly CallResult<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetBankAccountsProtocolEndpoint(CallResult<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetDepositsProtocolEndpoint : IGetDepositsProtocolEndpoint
{
    private readonly Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetDepositsProtocolEndpoint(Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(count, before, after));
    }
}

internal sealed class FakeGetChildOrdersProtocolEndpoint : IGetChildOrdersProtocolEndpoint
{
    private readonly Func<string?, int?, long?, long?, string?, string?, string?, string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetChildOrdersProtocolEndpoint(Func<string?, int?, long?, long?, string?, string?, string?, string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId));
    }
}

internal sealed class FakeWithdrawProtocolEndpoint : IWithdrawProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeWithdrawProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeGetExecutionsProtocolEndpoint : IGetExecutionsProtocolEndpoint
{
    private readonly Func<string, int?, long?, long?, string?, string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetExecutionsProtocolEndpoint(Func<string, int?, long?, long?, string?, string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode, count, before, after, childOrderId, childOrderAcceptanceId));
    }
}

internal sealed class FakeGetWithdrawalsProtocolEndpoint : IGetWithdrawalsProtocolEndpoint
{
    private readonly Func<int?, long?, long?, string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetWithdrawalsProtocolEndpoint(Func<int?, long?, long?, string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        string? messageId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(count, before, after, messageId));
    }
}

internal sealed class FakeGetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
{
    private readonly Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetCollateralHistoryProtocolEndpoint(Func<int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(count, before, after));
    }
}

internal sealed class FakeGetParentOrdersProtocolEndpoint : IGetParentOrdersProtocolEndpoint
{
    private readonly Func<string?, int?, long?, long?, string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetParentOrdersProtocolEndpoint(Func<string?, int?, long?, long?, string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode, count, before, after, parentOrderState));
    }
}

internal sealed class FakeGetParentOrderProtocolEndpoint : IGetParentOrderProtocolEndpoint
{
    private readonly Func<string?, string?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetParentOrderProtocolEndpoint(Func<string?, string?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(parentOrderId, parentOrderAcceptanceId));
    }
}

internal sealed class FakeGetBalanceHistoryProtocolEndpoint : IGetBalanceHistoryProtocolEndpoint
{
    private readonly Func<string?, int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetBalanceHistoryProtocolEndpoint(Func<string?, int?, long?, long?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? currencyCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(currencyCode, count, before, after));
    }
}

internal sealed class FakeGetPositionsProtocolEndpoint : IGetPositionsProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetPositionsProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetTradingCommissionProtocolEndpoint : IGetTradingCommissionProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetTradingCommissionProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeSendParentOrderProtocolEndpoint : ISendParentOrderProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeSendParentOrderProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeSendChildOrderProtocolEndpoint : ISendChildOrderProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeSendChildOrderProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeCancelParentOrderProtocolEndpoint : ICancelParentOrderProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeCancelParentOrderProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeCancelAllChildOrdersProtocolEndpoint : ICancelAllChildOrdersProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeCancelAllChildOrdersProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeCancelChildOrderProtocolEndpoint : ICancelChildOrderProtocolEndpoint
{
    private readonly Func<string, CallResult<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeCancelChildOrderProtocolEndpoint(Func<string, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}
