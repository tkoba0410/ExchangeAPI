using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

internal sealed class FakeProtocolTransport : IProtocolTransport
{
    public ProtocolRequest? LastRequest { get; private set; }
    public ProtocolTransportAuthMode LastAuthMode { get; private set; }
    public IApiCredentialSession? LastCredentialSession { get; private set; }
    public Func<ProtocolRequest, ProtocolTransportAuthMode, ProtocolTransportResult>? Handler { get; set; }

    public Task<ProtocolTransportResult> SendAsync(
        ProtocolRequest request,
        ProtocolTransportAuthMode authMode,
        CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        LastAuthMode = authMode;
        LastCredentialSession = null;

        var result = Handler?.Invoke(request, authMode) ?? new ProtocolTransportResult
        {
            IsSuccess = true,
            Response = new ProtocolResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string[]>(),
                BodyText = "{}",
            },
        };

        return Task.FromResult(result);
    }

    public Task<ProtocolTransportResult> SendAsync(
        ProtocolRequest request,
        ProtocolTransportAuthMode authMode,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        LastRequest = request;
        LastAuthMode = authMode;
        LastCredentialSession = credentialSession;

        var result = Handler?.Invoke(request, authMode) ?? new ProtocolTransportResult
        {
            IsSuccess = true,
            Response = new ProtocolResponse
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string[]>(),
                BodyText = "{}",
            },
        };

        return Task.FromResult(result);
    }
}
