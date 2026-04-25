using ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Binance.Native.Tests.Fakes;

internal sealed class FakeGetKlinesProtocolEndpoint : IGetKlinesProtocolEndpoint
{
    private readonly Func<string, string, long?, long?, string?, int?, CallResult<ProtocolRequest, ProtocolResponse>> _handler;

    public FakeGetKlinesProtocolEndpoint(Func<string, string, long?, long?, string?, int?, CallResult<ProtocolRequest, ProtocolResponse>> handler)
    {
        _handler = handler;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string symbol,
        string interval,
        long? startTime = null,
        long? endTime = null,
        string? timeZone = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_handler(symbol, interval, startTime, endTime, timeZone, limit));
    }
}
