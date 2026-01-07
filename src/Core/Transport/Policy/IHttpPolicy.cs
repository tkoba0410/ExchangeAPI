using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Core.Transport.Policy;

/// <summary>
/// HTTP 呼び出しにリトライやタイムアウトなどのポリシーを適用する拡張ポイント。
/// </summary>
public interface IHttpPolicy
{
    Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default);
}
