using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Transport.Policy;

/// <summary>
/// 複数のポリシーを順に適用するコンポジション。
/// </summary>
public sealed class HttpPolicyPipeline : IHttpPolicy
{
    private readonly IReadOnlyList<IHttpPolicy> _policies;

    public HttpPolicyPipeline(params IHttpPolicy[] policies)
    {
        _policies = policies ?? throw new ArgumentNullException(nameof(policies));
    }

    public Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (sendAsync is null) throw new ArgumentNullException(nameof(sendAsync));

        return ExecuteInternal(0, request, sendAsync, cancellationToken);
    }

    private Task<HttpResponseMessage> ExecuteInternal(
        int index,
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken)
    {
        if (index >= _policies.Count)
        {
            return sendAsync(cancellationToken);
        }

        return _policies[index].ExecuteAsync(
            request,
            ct => ExecuteInternal(index + 1, request, sendAsync, ct),
            cancellationToken);
    }
}
