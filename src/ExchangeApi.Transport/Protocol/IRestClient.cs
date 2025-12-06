using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Transport.Protocol;

/// <summary>
/// JSON ベースの REST API を呼び出すための最小限のクライアント インターフェース。
/// Stage1 では GET + JSON デシリアライズのみをサポートする。
/// </summary>
public interface IRestClient
{
    /// <summary>
    /// 指定されたパスに対して HTTP GET を実行し、
    /// JSON レスポンスを <typeparamref name="TResponse"/> にデシリアライズする。
    /// </summary>
    /// <typeparam name="TResponse">レスポンス JSON をマッピングする型。</typeparam>
    /// <param name="path">ベース URL からの相対パス。</param>
    /// <param name="query">クエリ文字列含む。</param>
    /// <param name="cancellationToken">キャンセル トークン。</param>
    /// <returns>デシリアライズされたレスポンス。</returns>
    Task<TResponse> GetAsync<TResponse>(
        string path,
        IReadOnlyDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 指定されたパスに対して HTTP POST を実行し、
    /// JSON レスポンスを <typeparamref name="TResponse"/> にデシリアライズする。
    /// </summary>
    Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        CancellationToken cancellationToken = default);
}
