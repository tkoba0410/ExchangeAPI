using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Transport.Transport;

/// <summary>
/// HTTP トランスポートの共通インターフェース。
/// プロトコル層（REST クライアントなど）から利用される想定。
/// </summary>
public interface IHttpTransport
{
    /// <summary>
    /// 指定された HTTP リクエストを送信します。
    /// </summary>
    /// <param name="request">送信する <see cref="HttpRequestMessage"/>。</param>
    /// <param name="cancellationToken">キャンセル トークン。</param>
    /// <returns>受信した <see cref="HttpResponseMessage"/>。</returns>
    Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);
}
