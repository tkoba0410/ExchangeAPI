using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Shared.Transport.Http;

/// <summary>
/// <see cref="HttpClient"/> を用いたデフォルト実装の HTTP トランスポート。
/// </summary>
public sealed class HttpTransport : IHttpTransport, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;
    private bool _disposed;

    /// <summary>
    /// <see cref="HttpClient"/> を指定して <see cref="HttpTransport"/> の新しいインスタンスを作成します。
    /// </summary>
    /// <param name="httpClient">内部で使用する <see cref="HttpClient"/>。</param>
    /// <param name="disposeHttpClient">
    /// このトランスポートの <see cref="Dispose"/> 呼び出し時に
    /// <paramref name="httpClient"/> も破棄する場合は true。
    /// DI コンテナ管理の <see cref="HttpClient"/> を渡す場合は false を推奨します。
    /// </param>
    public HttpTransport(HttpClient httpClient, bool disposeHttpClient = false)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _disposeHttpClient = disposeHttpClient;
    }

    /// <summary>
    /// <see cref="HttpMessageHandler"/> を指定して内部で <see cref="HttpClient"/> を生成する。
    /// </summary>
    /// <param name="handler">HTTP ハンドラ。Keep-Alive や Proxy 設定などを含めてカスタマイズ可能。</param>
    /// <param name="disposeHandler">Dispose 時にハンドラも破棄する場合は true。</param>
    public HttpTransport(HttpMessageHandler handler, bool disposeHandler = true)
    {
        if (handler is null) throw new ArgumentNullException(nameof(handler));
        _httpClient = new HttpClient(handler, disposeHandler);
        _disposeHttpClient = true;
    }

    /// <inheritdoc />
    public Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        ThrowIfDisposed();

        // ヘッダー読み取り完了時点で制御を返すことで、
        // 呼び出し側がレスポンスボディの読み取り方法を選択しやすくする。
        return _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(HttpTransport));
        }
    }

    /// <summary>
    /// 必要であれば内部の <see cref="HttpClient"/> を破棄します。
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}
