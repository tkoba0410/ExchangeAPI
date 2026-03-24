using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

internal sealed class FakeGetMarketsProtocolEndpoint : IGetMarketsProtocolEndpoint
{
    private readonly Func<Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetMarketsProtocolEndpoint(Func<Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler());
    }
}

internal sealed class FakeGetBoardProtocolEndpoint : IGetBoardProtocolEndpoint
{
    private readonly Func<string?, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetBoardProtocolEndpoint(Func<string?, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetExecutionsPublicProtocolEndpoint : IGetExecutionsPublicProtocolEndpoint
{
    private readonly Func<string?, int?, long?, long?, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetExecutionsPublicProtocolEndpoint(Func<string?, int?, long?, long?, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
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
    private readonly Func<string?, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetTickerProtocolEndpoint(Func<string?, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string? productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
{
    private readonly Call<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetBalanceProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetCollateralProtocolEndpoint : IGetCollateralProtocolEndpoint
{
    private readonly Call<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetCollateralProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetCollateralAccountsProtocolEndpoint : IGetCollateralAccountsProtocolEndpoint
{
    private readonly Call<ProtocolRequest, ProtocolResponse> _call;

    public FakeGetCollateralAccountsProtocolEndpoint(Call<ProtocolRequest, ProtocolResponse> call)
    {
        _call = call;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_call);
    }
}

internal sealed class FakeGetChildOrdersProtocolEndpoint : IGetChildOrdersProtocolEndpoint
{
    private readonly Func<string?, int?, long?, long?, string?, string?, string?, string?, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetChildOrdersProtocolEndpoint(Func<string?, int?, long?, long?, string?, string?, string?, string?, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
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

internal sealed class FakeGetExecutionsProtocolEndpoint : IGetExecutionsProtocolEndpoint
{
    private readonly Func<string, int?, long?, long?, string?, string?, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetExecutionsProtocolEndpoint(Func<string, int?, long?, long?, string?, string?, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
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

internal sealed class FakeGetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
{
    private readonly Func<int?, long?, long?, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetCollateralHistoryProtocolEndpoint(Func<int?, long?, long?, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(count, before, after));
    }
}

internal sealed class FakeGetPositionsProtocolEndpoint : IGetPositionsProtocolEndpoint
{
    private readonly Func<string, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetPositionsProtocolEndpoint(Func<string, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeGetTradingCommissionProtocolEndpoint : IGetTradingCommissionProtocolEndpoint
{
    private readonly Func<string, Call<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetTradingCommissionProtocolEndpoint(Func<string, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(productCode));
    }
}

internal sealed class FakeSendChildOrderProtocolEndpoint : ISendChildOrderProtocolEndpoint
{
    private readonly Func<string, Call<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeSendChildOrderProtocolEndpoint(Func<string, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeCancelAllChildOrdersProtocolEndpoint : ICancelAllChildOrdersProtocolEndpoint
{
    private readonly Func<string, Call<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeCancelAllChildOrdersProtocolEndpoint(Func<string, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}

internal sealed class FakeCancelChildOrderProtocolEndpoint : ICancelChildOrderProtocolEndpoint
{
    private readonly Func<string, Call<ProtocolRequest, ProtocolResponse>> _handler;
    public string? LastBodyJson { get; private set; }

    public FakeCancelChildOrderProtocolEndpoint(Func<string, Call<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(string bodyJson, CancellationToken cancellationToken = default)
    {
        LastBodyJson = bodyJson;
        return Task.FromResult(_handler(bodyJson));
    }
}
