using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

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
