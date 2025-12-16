using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core.Transport.Protocol;
using Core.Transport.Http;
namespace Exchange.Bitflyer.Raw;

public sealed class BitflyerSigningTransport : IHttpTransport
{
    private readonly IHttpTransport _inner;
    private readonly IRequestSigner _signer;

    public BitflyerSigningTransport(IHttpTransport inner, IRequestSigner signer)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _signer = signer ?? throw new ArgumentNullException(nameof(signer));
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        await _signer.SignAsync(request, cancellationToken).ConfigureAwait(false);
        return await _inner.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
