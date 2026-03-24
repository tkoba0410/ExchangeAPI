using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Binance.Protocol.Internal.Runtime;

public sealed class ProtocolTransportResult
{
    public required bool IsSuccess { get; init; }
    public ProtocolResponse? Response { get; init; }
    public CallError? Error { get; init; }
}
