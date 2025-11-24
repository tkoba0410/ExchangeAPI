using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Infrastructure.Protocol;

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
    /// <param name="relativePath">ベース URL からの相対パス（クエリ文字列含む）。</param>
    /// <param name="cancellationToken">キャンセル トークン。</param>
    /// <returns>デシリアライズされたレスポンス。</returns>
    Task<TResponse> GetAsync<TResponse>(
        string relativePath,
        CancellationToken cancellationToken = default);
}
