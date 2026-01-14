using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Transport;
namespace ExchangeApi.Core.Transport.Protocol;

/// <summary>
/// REST API を呼び出すための最小限のクライアント インターフェース。
/// transport は JSON を解釈しない（TopSpec 9）。
/// </summary>
public interface IRestClient
{
    /// <summary>
    /// 指定されたパスに対して HTTP GET を実行し、レスポンスの生データを返す。
    /// </summary>
    Task<HttpResponseMeta> GetRawAsync(
        string path,
        IReadOnlyDictionary<string, string?>? query = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 任意メソッドで HTTP を実行し、レスポンスの生データを返す。
    /// HTTP ステータス(4xx/5xx)では例外を投げず、Raw 層での解釈に委ねる。
    /// 例外化するのは transport レベル（接続失敗、タイムアウト、TLS、キャンセル等）のみ。
    /// </summary>
    Task<HttpResponseMeta> SendRawAsync(
        string method,
        string path,
        string? query = null,
        string? bodyJson = null,
        IReadOnlyDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default);
}
