using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;

namespace ExchangeApi.Spec.Wire;

public interface IWireTransport
{
    Task<WireCall> SendAsync(
        ExchangeCode exchange,
        WireRequest request,
        CancellationToken ct = default);
}
