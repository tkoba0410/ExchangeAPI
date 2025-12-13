using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Transport.Policy;

/// <summary>
/// 最大実行時間を設けるポリシー。
/// </summary>
public sealed class TimeoutHttpPolicy : IHttpPolicy
{
    private readonly TimeSpan _timeout;

    public TimeoutHttpPolicy(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(timeout));
        _timeout = timeout;
    }

    public Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        if (sendAsync is null) throw new ArgumentNullException(nameof(sendAsync));

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(_timeout);

        return ExecuteWithTimeoutAsync(sendAsync, linkedCts);
    }

    private static async Task<HttpResponseMessage> ExecuteWithTimeoutAsync(
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationTokenSource cts)
    {
        using (cts)
        {
            return await sendAsync(cts.Token).ConfigureAwait(false);
        }
    }
}
