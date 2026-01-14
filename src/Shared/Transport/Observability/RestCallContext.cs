using System;
using System.Net.Http;
namespace ExchangeApi.Shared.Transport.Observability;

/// <summary>
/// REST 呼び出しの観測コンテキスト。
/// </summary>
public sealed class RestCallContext
{
    public Guid RequestId { get; }
    public HttpRequestMessage Request { get; }
    public string Endpoint { get; }
    public string Method { get; }
    public string? ProductCode { get; }

    public RestCallContext(HttpRequestMessage request, string? productCode = null)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
        RequestId = Guid.NewGuid();
        Method = request.Method.Method;
        Endpoint = request.RequestUri?.AbsolutePath ?? string.Empty;
        ProductCode = productCode;
    }
}
