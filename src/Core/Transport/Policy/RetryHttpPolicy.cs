using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core.Contracts.Errors;
namespace Core.Transport.Policy;

/// <summary>
/// 一時障害やレートリミットに対してリトライを行うポリシー。
/// </summary>
public sealed class RetryHttpPolicy : IHttpPolicy
{
    private readonly int _maxAttemptsForGet;
    private readonly int _maxAttemptsForOther;
    private readonly TimeSpan _baseDelay;
    private readonly TimeSpan _maxDelay;
    private readonly IPolicyObserver _observer;

    public RetryHttpPolicy(int maxAttemptsForGet, int maxAttemptsForOther, TimeSpan baseDelay, TimeSpan maxDelay, IPolicyObserver? observer = null)
    {
        if (maxAttemptsForGet < 1) throw new ArgumentOutOfRangeException(nameof(maxAttemptsForGet));
        if (maxAttemptsForOther < 1) throw new ArgumentOutOfRangeException(nameof(maxAttemptsForOther));
        if (baseDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(baseDelay));
        if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay));

        _maxAttemptsForGet = maxAttemptsForGet;
        _maxAttemptsForOther = maxAttemptsForOther;
        _baseDelay = baseDelay;
        _maxDelay = maxDelay;
        _observer = observer ?? NoOpPolicyObserver.Instance;
    }

    public async Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (sendAsync is null) throw new ArgumentNullException(nameof(sendAsync));

        var maxAttempts = GetMaxAttempts(request.Method);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var response = await sendAsync(cancellationToken).ConfigureAwait(false);

                if (ShouldRetryResponse(response))
                {
                    var delay = GetDelay(attempt);
                    _observer.OnRetry(request, attempt, maxAttempts, delay, null, response);
                    response.Dispose();
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                return response;
            }
            catch (Exception ex) when (ShouldRetryException(ex, cancellationToken) && attempt < maxAttempts)
            {
                var delay = GetDelay(attempt);
                _observer.OnRetry(request, attempt, maxAttempts, delay, ex, null);
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                continue;
            }
        }

        // 最終試行の結果を返却（リトライ条件を満たさないか、最後の成功レスポンス）
        return await sendAsync(cancellationToken).ConfigureAwait(false);
    }

    private int GetMaxAttempts(HttpMethod method)
    {
        return method == HttpMethod.Get ? _maxAttemptsForGet : _maxAttemptsForOther;
    }

    private static bool ShouldRetryResponse(HttpResponseMessage response)
    {
        if (response is null) return false;

        var status = response.StatusCode;
        return status == HttpStatusCode.TooManyRequests
               || (int)status >= 500;
    }

    private static bool ShouldRetryException(Exception exception, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return false;

        return exception switch
        {
            HttpRequestException => true,
            TaskCanceledException => true,
            ExchangeApiException apiEx => apiEx.ErrorCategory is ExchangeErrorCategory.RateLimit
                or ExchangeErrorCategory.Network
                or ExchangeErrorCategory.Server,
            _ => false
        };
    }

    private TimeSpan GetDelay(int attempt)
    {
        var delay = TimeSpan.FromMilliseconds(_baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
        if (delay > _maxDelay) delay = _maxDelay;
        return delay;
    }
}
