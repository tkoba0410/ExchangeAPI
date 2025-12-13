using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Transport.Policy;

/// <summary>
/// ポリシー適用を行わず、そのまま送信するデフォルト実装。
/// </summary>
public sealed class NoOpHttpPolicy : IHttpPolicy
{
    public static readonly NoOpHttpPolicy Instance = new();

    private NoOpHttpPolicy() { }

    public Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        if (sendAsync is null) throw new ArgumentNullException(nameof(sendAsync));
        return sendAsync(cancellationToken);
    }
}
